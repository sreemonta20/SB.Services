using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SBERP.HumanResources.Service
{
    internal static class BiometricCommon
    {
        public const string PurposePunch = "punch";
        public const string PurposeVerify = "verify";

        /// <summary>BIO-FP-EMP-0001-000001: type, employee code and the next number of SEQ_BiometricRegistrationNo.</summary>
        public static async Task<string> NextRegistrationNoAsync(HumanResourcesDBContext ctx, int biometricTypeId, string? employeeCode)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT NEXT VALUE FOR dbo.{HumanResourcesDBContext.BiometricRegistrationSequence}";
            if (ctx.Database.CurrentTransaction != null)
                cmd.Transaction = ctx.Database.CurrentTransaction.GetDbTransaction();
            long seq = Convert.ToInt64(await cmd.ExecuteScalarAsync());
            string type = biometricTypeId == (int)EnumBiometricType.Face ? "FC" : "FP";
            string code = Regex.Replace((employeeCode ?? "").ToUpperInvariant(), "[^A-Z0-9-]", "");
            if (code.Length > 30) code = code[..30];
            return $"BIO-{type}-{code}-{seq:D6}";
        }

        public static bool EmployeeCanPunch(Employee? e)
            => e != null && e.IsActive == true &&
               (e.EmploymentStatus == null || e.EmploymentStatus <= (int)EnumEmploymentStatus.OnLeave);

        public static string HashKey(string key) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key)));
    }

    // =========================================================================================
    // Registration
    // =========================================================================================
    public class BiometricRegistrationService : IBiometricRegistrationService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly IBiometricVault _vault;
        private readonly IBiometricChallengeStore _challenges;

        public BiometricRegistrationService(HumanResourcesDBContext ctx, IHRLogService log, IBiometricVault vault, IBiometricChallengeStore challenges)
        { _ctx = ctx; _log = log; _vault = vault; _challenges = challenges; }

        private async Task<Employee?> EmployeeOfUserAsync(string? userId)
            => Guid.TryParse(userId, out var uid)
                ? await _ctx.Employees!.FirstOrDefaultAsync(e => e.AppUserId == uid && e.IsActive == true)
                : null;

        public async Task<DataResponse> RegisterMobileAsync(MobileBiometricRegistrationRequest request, string? userId)
        {
            string? savedFile = null;
            try
            {
                if (!request.ConsentGiven) return Utilities.Warn(ConstantSupplier.BIO_CONSENT_REQUIRED);
                if (!DeviceSignature.IsValidPublicKey(request.PublicKey)) return Utilities.Warn(ConstantSupplier.BIO_INVALID_PUBLIC_KEY);

                // the employee ID typed in the app must be the logged-in user's own employee record
                var emp = await EmployeeOfUserAsync(userId);
                if (emp == null || !string.Equals(emp.EmployeeCode, request.EmployeeCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                    return Utilities.Warn(ConstantSupplier.BIO_NOT_YOUR_EMPLOYEE, code: (int)HttpStatusCode.Forbidden);
                if (!BiometricCommon.EmployeeCanPunch(emp)) return Utilities.Warn(ConstantSupplier.BIO_EMPLOYEE_NOT_FOUND);

                byte[]? templateBytes = null;
                if (request.Templates is { Count: > 0 })
                {
                    templateBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request.Templates));
                    if (templateBytes.Length > _vault.MaxBytes) return Utilities.Warn(ConstantSupplier.BIO_TEMPLATE_TOO_LARGE);
                }

                await using var tx = await _ctx.Database.BeginTransactionAsync();
                DateTime now = DateTime.UtcNow;
                string actor = emp.Id.ToString();

                // one phone per employee: a new registration replaces the previous one
                var previous = await _ctx.EmployeeBiometricRegistrations!
                    .Where(r => r.EmployeeId == emp.Id && r.MethodId == (int)EnumBiometricMethod.MobileDeviceKey &&
                                r.StatusId != (int)EnumBiometricRegistrationStatus.Revoked)
                    .ToListAsync();
                foreach (var r in previous)
                {
                    r.StatusId = (int)EnumBiometricRegistrationStatus.Revoked;
                    r.RevokedDate = now; r.RevokedReason = "Replaced by a new device registration";
                    r.UpdatedBy = actor; r.UpdatedDate = now; r.IsActive = false;
                }

                var reg = new EmployeeBiometricRegistration
                {
                    Id = Guid.NewGuid(),
                    RegistrationNo = await BiometricCommon.NextRegistrationNoAsync(_ctx, request.BiometricTypeId, emp.EmployeeCode),
                    EmployeeId = emp.Id,
                    BiometricTypeId = request.BiometricTypeId,
                    MethodId = (int)EnumBiometricMethod.MobileDeviceKey,
                    StatusId = (int)EnumBiometricRegistrationStatus.PendingVerification,
                    DeviceInstallationId = request.DeviceInstallationId, DevicePlatform = request.DevicePlatform,
                    DeviceModel = request.DeviceModel, AppVersion = request.AppVersion,
                    PublicKey = request.PublicKey, KeyAlgorithm = "ES256",
                    ConsentGiven = true, ConsentDate = now,
                    CreatedBy = actor, CreatedDate = now, IsActive = true
                };
                if (templateBytes != null)
                {
                    var (fileName, sha, size) = await _vault.SaveAsync(reg.RegistrationNo, templateBytes);
                    savedFile = fileName;
                    reg.TemplateFileName = fileName; reg.TemplateSha256 = sha; reg.TemplateSizeBytes = size;
                    reg.TemplateFormat = request.Templates!.Select(t => t.Format).FirstOrDefault(f => !string.IsNullOrWhiteSpace(f));
                    reg.CapturedAngles = string.Join(",", request.Templates!.Select(t => t.Angle).Where(a => !string.IsNullOrWhiteSpace(a)));
                }
                await _ctx.EmployeeBiometricRegistrations!.AddAsync(reg);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                savedFile = null;

                var (id, nonce) = await _challenges.IssueAsync(reg.RegistrationNo, BiometricCommon.PurposeVerify);
                return Utilities.Ok(ConstantSupplier.BIO_REGISTERED, new BiometricChallengeResponse
                {
                    RegistrationNo = reg.RegistrationNo, ChallengeId = id, Challenge = nonce, ExpiresInSeconds = _challenges.LifetimeSeconds
                });
            }
            catch (Exception ex)
            {
                if (savedFile != null) await _vault.DeleteAsync(savedFile);
                return Utilities.Exception(ex, _log, nameof(RegisterMobileAsync));
            }
        }

        public async Task<DataResponse> VerifyRegistrationAsync(VerifyBiometricRegistrationRequest request, string? userId)
        {
            try
            {
                var challenge = await _challenges.ConsumeAsync(request.ChallengeId);
                if (challenge == null || challenge.Value.registrationNo != request.RegistrationNo ||
                    challenge.Value.purpose != BiometricCommon.PurposeVerify)
                    return Utilities.Warn(ConstantSupplier.BIO_CHALLENGE_INVALID);

                var emp = await EmployeeOfUserAsync(userId);
                var reg = await _ctx.EmployeeBiometricRegistrations!.FirstOrDefaultAsync(r =>
                    r.RegistrationNo == request.RegistrationNo && r.MethodId == (int)EnumBiometricMethod.MobileDeviceKey &&
                    r.StatusId == (int)EnumBiometricRegistrationStatus.PendingVerification);
                if (reg == null || emp == null || reg.EmployeeId != emp.Id)
                    return Utilities.Warn(ConstantSupplier.BIO_REGISTRATION_INVALID);

                string message = DeviceSignature.Message(challenge.Value.nonce, reg.RegistrationNo!, BiometricCommon.PurposeVerify, null);
                if (!DeviceSignature.Verify(reg.PublicKey, message, request.Signature))
                    return Utilities.Warn(ConstantSupplier.BIO_SIGNATURE_INVALID, code: (int)HttpStatusCode.Unauthorized);

                reg.StatusId = (int)EnumBiometricRegistrationStatus.Active;
                reg.VerifiedDate = DateTime.UtcNow;
                reg.UpdatedBy = emp.Id.ToString(); reg.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.BIO_VERIFIED, new
                {
                    reg.RegistrationNo, reg.BiometricTypeId, emp.EmployeeCode, EmployeeName = emp.FullName
                });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(VerifyRegistrationAsync)); }
        }

        public async Task<DataResponse> GetMyRegistrationAsync(string? userId)
        {
            try
            {
                var emp = await EmployeeOfUserAsync(userId);
                if (emp == null) return Utilities.Warn(ConstantSupplier.BIO_NOT_YOUR_EMPLOYEE, code: (int)HttpStatusCode.NotFound);
                var reg = await _ctx.EmployeeBiometricRegistrations!.AsNoTracking()
                    .Where(r => r.EmployeeId == emp.Id && r.MethodId == (int)EnumBiometricMethod.MobileDeviceKey &&
                                r.StatusId != (int)EnumBiometricRegistrationStatus.Revoked)
                    .OrderByDescending(r => r.CreatedDate)
                    .Select(r => new { r.RegistrationNo, r.BiometricTypeId, r.StatusId, r.DeviceInstallationId, r.VerifiedDate })
                    .FirstOrDefaultAsync();
                return Utilities.Ok(ConstantSupplier.BIO_FETCH_SUCCESS, new { emp.EmployeeCode, EmployeeName = emp.FullName, Registration = reg });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetMyRegistrationAsync)); }
        }

        public async Task<PagingResult<BiometricRegistrationResponse>?> GetAllRegistrationsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<BiometricRegistrationResponse>(_ctx, ConstantSupplier.SP_GET_ALL_BIOMETRIC_REGISTRATIONS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllRegistrationsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateTerminalEnrollmentAsync(TerminalEnrollmentRequest request)
        {
            try
            {
                if (!request.ConsentGiven) return Utilities.Warn(ConstantSupplier.BIO_CONSENT_REQUIRED);
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                Guid? deviceId = Guid.TryParse(request.AttendanceDeviceId, out var d) ? d : null;
                string pin = request.DeviceUserPin!.Trim();

                var emp = await _ctx.Employees!.FirstOrDefaultAsync(e => e.Id == empId);
                if (!BiometricCommon.EmployeeCanPunch(emp)) return Utilities.Warn(ConstantSupplier.BIO_EMPLOYEE_NOT_FOUND);

                var sameTerminalScope = _ctx.EmployeeBiometricRegistrations!.Where(r =>
                    r.MethodId == (int)EnumBiometricMethod.DoorTerminal && r.StatusId == (int)EnumBiometricRegistrationStatus.Active &&
                    r.DeviceUserPin == pin && (r.AttendanceDeviceId == null || deviceId == null || r.AttendanceDeviceId == deviceId));
                if (await sameTerminalScope.AnyAsync(r => r.EmployeeId != empId)) return Utilities.Warn(ConstantSupplier.BIO_PIN_IN_USE);
                if (await sameTerminalScope.AnyAsync(r => r.EmployeeId == empId && r.BiometricTypeId == request.BiometricTypeId))
                    return Utilities.Ok(ConstantSupplier.BIO_TERMINAL_ENROLLED);

                await using var tx = await _ctx.Database.BeginTransactionAsync();
                DateTime now = DateTime.UtcNow;
                var reg = new EmployeeBiometricRegistration
                {
                    Id = Guid.NewGuid(),
                    RegistrationNo = await BiometricCommon.NextRegistrationNoAsync(_ctx, request.BiometricTypeId, emp!.EmployeeCode),
                    EmployeeId = empId, BiometricTypeId = request.BiometricTypeId,
                    MethodId = (int)EnumBiometricMethod.DoorTerminal, StatusId = (int)EnumBiometricRegistrationStatus.Active,
                    AttendanceDeviceId = deviceId, DeviceUserPin = pin,
                    ConsentGiven = true, ConsentDate = now, VerifiedDate = now,
                    CreatedBy = request.CreateUpdateBy, CreatedDate = now, IsActive = true
                };
                await _ctx.EmployeeBiometricRegistrations!.AddAsync(reg);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.BIO_TERMINAL_ENROLLED, new { reg.Id, reg.RegistrationNo });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateTerminalEnrollmentAsync)); }
        }

        public async Task<DataResponse> RevokeRegistrationAsync(RevokeBiometricRegistrationRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid registration id");
                var reg = await _ctx.EmployeeBiometricRegistrations!.FirstOrDefaultAsync(r => r.Id == id);
                if (reg == null) return Utilities.Warn(ConstantSupplier.BIO_REGISTRATION_INVALID, code: (int)HttpStatusCode.NotFound);

                // biometric data is kept only while it is in use
                await _vault.DeleteAsync(reg.TemplateFileName);
                reg.TemplateFileName = null; reg.TemplateSha256 = null; reg.TemplateSizeBytes = null;
                reg.StatusId = (int)EnumBiometricRegistrationStatus.Revoked;
                reg.RevokedDate = DateTime.UtcNow;
                reg.RevokedReason = string.IsNullOrWhiteSpace(request.Reason) ? "Revoked by HR" : request.Reason!.Trim()[..Math.Min(200, request.Reason!.Trim().Length)];
                reg.UpdatedBy = request.CreateUpdateBy; reg.UpdatedDate = DateTime.UtcNow; reg.IsActive = false;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.BIO_REVOKED);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(RevokeRegistrationAsync)); }
        }
    }

    // =========================================================================================
    // Punches: Identification API (mobile), door terminals, punch log
    // =========================================================================================
    public class AttendancePunchService : IAttendancePunchService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly IBiometricChallengeStore _challenges;
        private readonly IAttendanceProcessor _processor;

        public AttendancePunchService(HumanResourcesDBContext ctx, IHRLogService log, IBiometricChallengeStore challenges, IAttendanceProcessor processor)
        { _ctx = ctx; _log = log; _challenges = challenges; _processor = processor; }

        private Task<HRSettings?> SettingsAsync() => _ctx.HRSettings!.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive == true);

        public async Task<DataResponse> GetChallengeAsync(BiometricChallengeRequest request)
        {
            try
            {
                string purpose = string.Equals(request.Purpose, BiometricCommon.PurposeVerify, StringComparison.OrdinalIgnoreCase)
                    ? BiometricCommon.PurposeVerify : BiometricCommon.PurposePunch;
                int neededStatus = purpose == BiometricCommon.PurposeVerify
                    ? (int)EnumBiometricRegistrationStatus.PendingVerification : (int)EnumBiometricRegistrationStatus.Active;

                bool exists = await _ctx.EmployeeBiometricRegistrations!.AnyAsync(r =>
                    r.RegistrationNo == request.RegistrationNo && r.MethodId == (int)EnumBiometricMethod.MobileDeviceKey && r.StatusId == neededStatus);
                if (!exists) return Utilities.Warn(ConstantSupplier.BIO_REGISTRATION_INVALID);

                var (id, nonce) = await _challenges.IssueAsync(request.RegistrationNo!, purpose);
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, new BiometricChallengeResponse
                {
                    RegistrationNo = request.RegistrationNo, ChallengeId = id, Challenge = nonce, ExpiresInSeconds = _challenges.LifetimeSeconds
                });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetChallengeAsync)); }
        }

        public async Task<DataResponse> MobilePunchAsync(MobilePunchRequest request, string? clientIp)
        {
            try
            {
                var settings = await SettingsAsync();
                if (settings?.MobileAttendanceEnabled == false)
                    return Utilities.Warn(ConstantSupplier.BIO_MOBILE_DISABLED, code: (int)HttpStatusCode.Forbidden);

                var clock = new AttendanceClock(settings);
                DateTime nowUtc = DateTime.UtcNow;
                DateTime local = clock.ToLocal(nowUtc);

                var reg = await _ctx.EmployeeBiometricRegistrations!.Include(r => r.Employee).FirstOrDefaultAsync(r =>
                    r.RegistrationNo == request.RegistrationNo && r.MethodId == (int)EnumBiometricMethod.MobileDeviceKey &&
                    r.StatusId == (int)EnumBiometricRegistrationStatus.Active);
                if (reg == null) return Utilities.Warn(ConstantSupplier.BIO_REGISTRATION_INVALID, code: (int)HttpStatusCode.Unauthorized);

                AttendancePunch NewPunch(int status, string? reason) => new()
                {
                    Id = Guid.NewGuid(), EmployeeId = reg.EmployeeId, PunchTimeUtc = nowUtc, PunchLocalTime = local,
                    AttendanceDate = clock.WorkDate(local), PunchTypeId = Math.Clamp(request.PunchTypeId, 0, 2),
                    SourceId = (int)EnumPunchSource.MobileApp, StatusId = status, RejectReason = reason,
                    BiometricTypeId = reg.BiometricTypeId, BiometricRegistrationId = reg.Id,
                    ClientRequestId = status == (int)EnumPunchStatus.Rejected ? null : request.ClientRequestId,
                    ClientTime = request.ClientTime, ClientIp = Trim(clientIp, 64),
                    Latitude = request.Latitude, Longitude = request.Longitude,
                    WifiBssid = Trim(request.WifiBssid, 64), WifiSsid = Trim(request.WifiSsid, 100),
                    CreatedBy = reg.EmployeeId.ToString(), CreatedDate = nowUtc
                };

                async Task<DataResponse> RejectAsync(string message, int code, bool? withinOffice = null)
                {
                    var p = NewPunch((int)EnumPunchStatus.Rejected, message);
                    p.IsWithinOffice = withinOffice;
                    await _ctx.AttendancePunches!.AddAsync(p);
                    await _ctx.SaveChangesAsync();
                    _log.LogWarning($"Mobile punch rejected for registration {reg.RegistrationNo}: {message}");
                    return Utilities.Warn(message, code: code);
                }

                // 1. the phone proved the employee's fingerprint / face by signing our one-time challenge
                var challenge = await _challenges.ConsumeAsync(request.ChallengeId);
                if (challenge == null || challenge.Value.registrationNo != reg.RegistrationNo || challenge.Value.purpose != BiometricCommon.PurposePunch)
                    return await RejectAsync(ConstantSupplier.BIO_CHALLENGE_INVALID, (int)HttpStatusCode.Unauthorized);
                string message = DeviceSignature.Message(challenge.Value.nonce, reg.RegistrationNo!, BiometricCommon.PurposePunch, request.ClientRequestId);
                if (!DeviceSignature.Verify(reg.PublicKey, message, request.Signature))
                    return await RejectAsync(ConstantSupplier.BIO_SIGNATURE_INVALID, (int)HttpStatusCode.Unauthorized);

                if (!BiometricCommon.EmployeeCanPunch(reg.Employee))
                    return await RejectAsync(ConstantSupplier.BIO_EMPLOYEE_NOT_FOUND, (int)HttpStatusCode.Forbidden);

                // 2. the same request again (app retried after a network error): answer with the stored result
                var earlier = await _ctx.AttendancePunches!.AsNoTracking().FirstOrDefaultAsync(p =>
                    p.SourceId == (int)EnumPunchSource.MobileApp && p.ClientRequestId == request.ClientRequestId);
                if (earlier != null)
                {
                    if (earlier.EmployeeId != reg.EmployeeId) return Utilities.Warn(ConstantSupplier.BIO_CHALLENGE_INVALID);
                    return await ResultAsync(reg.Employee!, earlier);
                }

                // 3. inside the office network
                string? rule = null;
                if (settings?.RequireOfficeNetwork != false)
                {
                    var zones = await _ctx.AttendanceZones!.AsNoTracking().Where(z => z.IsActive == true).ToListAsync();
                    if (zones.Count == 0) return await RejectAsync(ConstantSupplier.BIO_NO_ZONE, (int)HttpStatusCode.Forbidden);
                    var (ok, passed) = OfficeNetwork.Check(zones, clientIp, request.WifiBssid, request.Latitude, request.Longitude);
                    if (!ok) return await RejectAsync(ConstantSupplier.BIO_OUTSIDE_OFFICE, (int)HttpStatusCode.Forbidden, false);
                    rule = passed;
                }

                // 4. a second tap within DuplicatePunchMinutes is kept but not counted
                int dupMinutes = settings?.DuplicatePunchMinutes ?? 1;
                DateTime dupFrom = nowUtc.AddMinutes(-dupMinutes);
                bool duplicate = dupMinutes > 0 && await _ctx.AttendancePunches!.AnyAsync(p =>
                    p.EmployeeId == reg.EmployeeId && p.StatusId == (int)EnumPunchStatus.Accepted && p.PunchTimeUtc >= dupFrom);

                var punch = NewPunch(duplicate ? (int)EnumPunchStatus.Duplicate : (int)EnumPunchStatus.Accepted, null);
                punch.IsWithinOffice = rule != null ? true : null;
                punch.LocationCheck = rule;
                await _ctx.AttendancePunches!.AddAsync(punch);
                reg.LastUsedDate = nowUtc;
                await _ctx.SaveChangesAsync();

                if (!duplicate) await _processor.RecalculateDayAsync(reg.EmployeeId, punch.AttendanceDate);
                return await ResultAsync(reg.Employee!, punch);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(MobilePunchAsync)); }
        }

        private async Task<DataResponse> ResultAsync(Employee emp, AttendancePunch punch)
        {
            var att = await _ctx.Attendances!.AsNoTracking()
                .FirstOrDefaultAsync(a => a.EmployeeId == emp.Id && a.AttendanceDate == punch.AttendanceDate);
            var first = await _ctx.AttendancePunches!.AsNoTracking()
                .Where(p => p.EmployeeId == emp.Id && p.AttendanceDate == punch.AttendanceDate && p.StatusId == (int)EnumPunchStatus.Accepted)
                .OrderBy(p => p.PunchLocalTime).Select(p => (Guid?)p.Id).FirstOrDefaultAsync();

            string result = punch.StatusId == (int)EnumPunchStatus.Duplicate ? "Duplicate"
                          : first == punch.Id ? "CheckedIn" : "CheckedOut";
            string msg = result switch
            {
                "Duplicate" => ConstantSupplier.BIO_PUNCH_DUPLICATE,
                "CheckedIn" => ConstantSupplier.BIO_PUNCH_OK_IN,
                _ => ConstantSupplier.BIO_PUNCH_OK_OUT
            };
            return Utilities.Ok(msg, new MobilePunchResponse
            {
                EmployeeCode = emp.EmployeeCode, EmployeeName = emp.FullName, Result = result,
                PunchTime = punch.PunchLocalTime, AttendanceDate = punch.AttendanceDate,
                CheckInTime = att?.CheckInTime?.ToString(@"hh\:mm"), CheckOutTime = att?.CheckOutTime?.ToString(@"hh\:mm")
            });
        }

        // ------------------------------------------------------------------ door terminals

        private sealed record TerminalPunch(string Pin, DateTime Local, int PunchTypeId, int? BiometricTypeId, string? Raw);

        private async Task<AttendanceDevice?> DeviceAsync(string? serialNumber, int protocol, string? clientIp)
        {
            if (string.IsNullOrWhiteSpace(serialNumber)) return null;
            var device = await _ctx.AttendanceDevices!.FirstOrDefaultAsync(d =>
                d.SerialNumber == serialNumber && d.Protocol == protocol && d.IsActive == true);
            if (device == null || (!string.IsNullOrWhiteSpace(device.IpAddress) && device.IpAddress != clientIp))
            {
                _log.LogWarning($"Attendance push refused: serial '{Trim(serialNumber, 50)}' from {clientIp}");
                return null;
            }
            return device;
        }

        /// <summary>Stores terminal punches (deduplicated per device + PIN + time) and recalculates the days.</summary>
        private async Task<(int stored, int skipped)> IngestTerminalAsync(AttendanceDevice device, List<TerminalPunch> items, string? clientIp)
        {
            var settings = await SettingsAsync();
            var clock = new AttendanceClock(settings);
            DateTime nowUtc = DateTime.UtcNow;
            device.LastSeenDate = nowUtc;
            if (items.Count == 0) { await _ctx.SaveChangesAsync(); return (0, 0); }

            var pins = items.Select(i => i.Pin).Distinct().ToList();
            var byRegistration = await _ctx.EmployeeBiometricRegistrations!.AsNoTracking()
                .Where(r => r.MethodId == (int)EnumBiometricMethod.DoorTerminal && r.StatusId == (int)EnumBiometricRegistrationStatus.Active &&
                            pins.Contains(r.DeviceUserPin!) && (r.AttendanceDeviceId == null || r.AttendanceDeviceId == device.Id))
                .Select(r => new { r.DeviceUserPin, r.EmployeeId, r.Id }).ToListAsync();
            var byCode = await _ctx.Employees!.AsNoTracking()
                .Where(e => pins.Contains(e.EmployeeCode!))
                .Select(e => new { e.EmployeeCode, e.Id, e.IsActive, e.EmploymentStatus }).ToListAsync();

            var requestIds = items.Select(i => RequestId(device, i)).ToList();
            var known = (await _ctx.AttendancePunches!.AsNoTracking()
                .Where(p => p.SourceId == (int)EnumPunchSource.DoorTerminal && requestIds.Contains(p.ClientRequestId!))
                .Select(p => p.ClientRequestId!).ToListAsync()).ToHashSet();

            int dupMinutes = settings?.DuplicatePunchMinutes ?? 1;
            var lastAccepted = new Dictionary<Guid, DateTime>();
            var recalc = new HashSet<(Guid, DateTime)>();
            int stored = 0, skipped = 0;

            foreach (var item in items.OrderBy(i => i.Local))
            {
                string requestId = RequestId(device, item);
                if (!known.Add(requestId)) { skipped++; continue; }

                DateTime utc = clock.ToUtc(item.Local);
                var reg = byRegistration.FirstOrDefault(r => r.DeviceUserPin == item.Pin);
                var emp = byCode.FirstOrDefault(e => string.Equals(e.EmployeeCode, item.Pin, StringComparison.OrdinalIgnoreCase));
                Guid? empId = reg?.EmployeeId ?? (emp != null && emp.IsActive == true ? emp.Id : null);

                int status = (int)EnumPunchStatus.Accepted; string? reason = null;
                if (empId == null) { status = (int)EnumPunchStatus.Rejected; reason = "Unknown terminal user PIN"; }
                else
                {
                    if (!lastAccepted.TryGetValue(empId.Value, out var prev))
                    {
                        DateTime from = utc.AddMinutes(-dupMinutes);
                        prev = await _ctx.AttendancePunches!.AsNoTracking()
                            .Where(p => p.EmployeeId == empId && p.StatusId == (int)EnumPunchStatus.Accepted && p.PunchTimeUtc >= from && p.PunchTimeUtc <= utc)
                            .OrderByDescending(p => p.PunchTimeUtc).Select(p => p.PunchTimeUtc).FirstOrDefaultAsync();
                    }
                    if (dupMinutes > 0 && prev != default && (utc - prev).TotalMinutes < dupMinutes && utc >= prev)
                        status = (int)EnumPunchStatus.Duplicate;
                    else lastAccepted[empId.Value] = utc;
                }

                var punch = new AttendancePunch
                {
                    Id = Guid.NewGuid(), EmployeeId = empId, PunchTimeUtc = utc, PunchLocalTime = item.Local,
                    AttendanceDate = clock.WorkDate(item.Local), PunchTypeId = item.PunchTypeId,
                    SourceId = (int)EnumPunchSource.DoorTerminal, StatusId = status, RejectReason = reason,
                    BiometricTypeId = item.BiometricTypeId, BiometricRegistrationId = reg?.Id,
                    AttendanceDeviceId = device.Id, DeviceUserPin = Trim(item.Pin, 30), ClientRequestId = requestId,
                    ClientIp = Trim(clientIp, 64), IsWithinOffice = true, LocationCheck = "DEVICE:" + device.DeviceCode,
                    RawData = Trim(item.Raw, 500), CreatedDate = nowUtc
                };
                await _ctx.AttendancePunches!.AddAsync(punch);
                stored++;
                if (status == (int)EnumPunchStatus.Accepted) recalc.Add((empId!.Value, punch.AttendanceDate));
                if (device.LastPunchDate == null || item.Local > device.LastPunchDate) device.LastPunchDate = item.Local;
            }

            await _ctx.SaveChangesAsync();
            foreach (var (empId, date) in recalc) await _processor.RecalculateDayAsync(empId, date);
            return (stored, skipped);
        }

        private static string RequestId(AttendanceDevice device, TerminalPunch p)
            => $"{device.SerialNumber}|{p.Pin}|{p.Local:yyyyMMddHHmmss}";

        public async Task<DataResponse> DevicePunchesAsync(DevicePunchBatchRequest request, string? deviceKey, string? clientIp)
        {
            try
            {
                var device = await DeviceAsync(request.SerialNumber, (int)EnumDeviceProtocol.GenericJson, clientIp);
                if (device == null || string.IsNullOrEmpty(deviceKey) || device.ApiKeyHash == null ||
                    !CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(BiometricCommon.HashKey(deviceKey)), Encoding.ASCII.GetBytes(device.ApiKeyHash)))
                    return Utilities.Warn(ConstantSupplier.DEVICE_UNKNOWN, code: (int)HttpStatusCode.Unauthorized);

                var items = (request.Punches ?? new()).Where(p => !string.IsNullOrWhiteSpace(p.UserPin))
                    .Select(p => new TerminalPunch(p.UserPin!.Trim(), DateTime.SpecifyKind(p.PunchTime, DateTimeKind.Unspecified),
                                                   Math.Clamp(p.PunchTypeId, 0, 2), p.BiometricTypeId is 1 or 2 ? p.BiometricTypeId : null,
                                                   p.DeviceLogId))
                    .ToList();
                var (stored, skipped) = await IngestTerminalAsync(device, items, clientIp);
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, new { Stored = stored, AlreadyReceived = skipped });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DevicePunchesAsync)); }
        }

        public async Task<string?> AdmsHandshakeAsync(string? serialNumber, string? clientIp)
        {
            var device = await DeviceAsync(serialNumber, (int)EnumDeviceProtocol.ZKTecoAdms, clientIp);
            if (device == null) return null;
            device.LastSeenDate = DateTime.UtcNow;
            await _ctx.SaveChangesAsync();

            var clock = new AttendanceClock(await SettingsAsync());
            int tzHours = (int)Math.Round(clock.TimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours);
            // ATTLOGStamp=None: the terminal sends every record it still holds; duplicates are ignored by RequestId
            return string.Join("\n",
                $"GET OPTION FROM: {device.SerialNumber}",
                "ATTLOGStamp=None", "OPERLOGStamp=9999", "ATTPHOTOStamp=None",
                "ErrorDelay=30", "Delay=10", "TransTimes=00:00;14:05", "TransInterval=1",
                "TransFlag=TransData AttLog", $"TimeZone={tzHours}", "Realtime=1", "Encrypt=None") + "\n";
        }

        public async Task<string?> AdmsUploadAsync(string? serialNumber, string? table, string body, string? clientIp)
        {
            var device = await DeviceAsync(serialNumber, (int)EnumDeviceProtocol.ZKTecoAdms, clientIp);
            if (device == null) return null;

            if (!string.Equals(table, "ATTLOG", StringComparison.OrdinalIgnoreCase))
            {
                // user / operation logs are not needed: acknowledge so the terminal moves on
                device.LastSeenDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return "OK";
            }

            // one line per punch: PIN \t yyyy-MM-dd HH:mm:ss \t status \t verify \t workcode ...
            var items = new List<TerminalPunch>();
            foreach (var line in body.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var f = line.Split('\t');
                if (f.Length < 2 || string.IsNullOrWhiteSpace(f[0]) ||
                    !DateTime.TryParseExact(f[1].Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
                    continue;
                int punchType = f.Length > 2 && int.TryParse(f[2], out var st) ? st switch { 0 or 3 or 4 => 1, 1 or 2 or 5 => 2, _ => 0 } : 0;
                int? bioType = f.Length > 3 && int.TryParse(f[3], out var vf) ? vf switch { 1 => 1, 15 => 2, _ => (int?)null } : null;
                items.Add(new TerminalPunch(f[0].Trim(), local, punchType, bioType, line));
            }
            var (stored, skipped) = await IngestTerminalAsync(device, items, clientIp);
            return $"OK: {stored + skipped}";
        }

        // ------------------------------------------------------------------ admin

        public async Task<PagingResult<AttendancePunchResponse>?> GetAllPunchesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<AttendancePunchResponse>(_ctx, ConstantSupplier.SP_GET_ALL_ATTENDANCE_PUNCHES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllPunchesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> ReprocessAsync(ReprocessAttendanceRequest request)
        {
            try
            {
                if (request.ToDate < request.FromDate || (request.ToDate - request.FromDate).TotalDays > 62)
                    return Utilities.Warn("Choose a date range of at most 62 days.");
                Guid? empId = Guid.TryParse(request.EmployeeId, out var g) ? g : null;
                int days = await _processor.RecalculateRangeAsync(request.FromDate, request.ToDate, empId);
                return Utilities.Ok(ConstantSupplier.ATT_REPROCESSED, new { EmployeeDays = days });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(ReprocessAsync)); }
        }

        private static string? Trim(string? s, int max) => s == null ? null : s.Length <= max ? s : s[..max];
    }

    // =========================================================================================
    // Devices and zones (admin CRUD)
    // =========================================================================================
    public class AttendanceDeviceService : IAttendanceDeviceService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public AttendanceDeviceService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<AttendanceDeviceResponse>?> GetAllDevicesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<AttendanceDeviceResponse>(_ctx, ConstantSupplier.SP_GET_ALL_ATTENDANCE_DEVICES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllDevicesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateDeviceAsync(AttendanceDeviceRequest request)
        {
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                Guid id = Guid.Empty;
                if (isUpdate && !Guid.TryParse(request.Id, out id)) return Utilities.Warn("Invalid device id");
                string code = request.DeviceCode!.Trim(), serial = request.SerialNumber!.Trim();

                if (await _ctx.AttendanceDevices!.AnyAsync(d => (d.DeviceCode == code || d.SerialNumber == serial) && (!isUpdate || d.Id != id)))
                    return Utilities.Warn(ConstantSupplier.DEVICE_DUPLICATE);
                if (!string.IsNullOrWhiteSpace(request.IpAddress) && !IPAddress.TryParse(request.IpAddress.Trim(), out _))
                    return Utilities.Warn("Invalid IP address.");

                AttendanceDevice? d;
                if (isUpdate)
                {
                    d = await _ctx.AttendanceDevices!.FirstOrDefaultAsync(x => x.Id == id);
                    if (d == null) return Utilities.Warn("Device not found", code: (int)HttpStatusCode.NotFound);
                    d.UpdatedBy = request.CreateUpdateBy; d.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    d = new AttendanceDevice { Id = Guid.NewGuid(), CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow };
                    await _ctx.AttendanceDevices!.AddAsync(d);
                }
                d.DeviceCode = code; d.Name = request.Name; d.Vendor = request.Vendor; d.Model = request.Model;
                d.SerialNumber = serial; d.Protocol = request.Protocol;
                d.BranchId = Guid.TryParse(request.BranchId, out var b) ? b : null;
                d.Location = request.Location; d.IpAddress = string.IsNullOrWhiteSpace(request.IpAddress) ? null : request.IpAddress.Trim();
                d.Remarks = request.Remarks; d.IsActive = request.IsActive;

                // the generic API authenticates with a device key; only its hash is stored, the key is shown once
                string? newKey = null;
                if (d.Protocol == (int)EnumDeviceProtocol.GenericJson && (!isUpdate || request.RegenerateKey || d.ApiKeyHash == null))
                {
                    newKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
                    d.ApiKeyHash = BiometricCommon.HashKey(newKey);
                }
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.DEVICE_SAVED, new { d.Id, DeviceKey = newKey });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateDeviceAsync)); }
        }

        public async Task<DataResponse> DeleteDeviceAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid device id");
                var d = await _ctx.AttendanceDevices!.FirstOrDefaultAsync(x => x.Id == g);
                if (d == null) return Utilities.Warn("Device not found", code: (int)HttpStatusCode.NotFound);
                d.IsActive = false; d.UpdatedDate = DateTime.UtcNow;   // soft delete: punches keep their device
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteDeviceAsync)); }
        }
    }

    public class AttendanceZoneService : IAttendanceZoneService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public AttendanceZoneService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<AttendanceZoneResponse>?> GetAllZonesPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<AttendanceZoneResponse>(_ctx, ConstantSupplier.SP_GET_ALL_ATTENDANCE_ZONES_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllZonesPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateZoneAsync(AttendanceZoneRequest request)
        {
            try
            {
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                Guid id = Guid.Empty;
                if (isUpdate && !Guid.TryParse(request.Id, out id)) return Utilities.Warn("Invalid zone id");
                string code = request.ZoneCode!.Trim();
                if (await _ctx.AttendanceZones!.AnyAsync(z => z.ZoneCode == code && (!isUpdate || z.Id != id)))
                    return Utilities.Warn(ConstantSupplier.ZONE_DUPLICATE);
                if (!OfficeNetwork.IsValidRangeList(request.AllowedIpRanges))
                    return Utilities.Warn("Invalid IP range. Use addresses like 192.168.1.0/24, separated by commas.");
                bool hasGps = request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusMeters > 0;
                if (string.IsNullOrWhiteSpace(request.AllowedIpRanges) && string.IsNullOrWhiteSpace(request.AllowedWifiBssids) && !hasGps)
                    return Utilities.Warn("Enter at least one IP range, Wi-Fi BSSID or GPS position with radius.");

                AttendanceZone? z;
                if (isUpdate)
                {
                    z = await _ctx.AttendanceZones!.FirstOrDefaultAsync(x => x.Id == id);
                    if (z == null) return Utilities.Warn("Zone not found", code: (int)HttpStatusCode.NotFound);
                    z.UpdatedBy = request.CreateUpdateBy; z.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    z = new AttendanceZone { Id = Guid.NewGuid(), CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow };
                    await _ctx.AttendanceZones!.AddAsync(z);
                }
                z.ZoneCode = code; z.Name = request.Name;
                z.BranchId = Guid.TryParse(request.BranchId, out var b) ? b : null;
                z.AllowedIpRanges = request.AllowedIpRanges?.Trim(); z.AllowedWifiBssids = request.AllowedWifiBssids?.Trim();
                z.Latitude = request.Latitude; z.Longitude = request.Longitude; z.RadiusMeters = request.RadiusMeters;
                z.Remarks = request.Remarks; z.IsActive = request.IsActive;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.ZONE_SAVED, new { z.Id });
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(CreateUpdateZoneAsync)); }
        }

        public async Task<DataResponse> DeleteZoneAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid zone id");
                var z = await _ctx.AttendanceZones!.FirstOrDefaultAsync(x => x.Id == g);
                if (z == null) return Utilities.Warn("Zone not found", code: (int)HttpStatusCode.NotFound);
                z.IsActive = false; z.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteZoneAsync)); }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using OfficeOpenXml;
using System.Net;

namespace SBERP.HumanResources.Service
{
    /// <summary>
    /// HRSettings is a single-row table — there should only ever be one active
    /// row. SaveUpdate finds that row (or creates it) and updates in place.
    ///
    /// Attendance Excel upload accepts rows of:
    ///   EmployeeCode | Date | CheckIn (HH:mm) | CheckOut (HH:mm) | Status
    /// Anything malformed is logged into AttendanceUploadLog.ErrorReport
    /// so HR can correct and re-upload without losing the audit trail.
    /// </summary>
    public class HRSettingsService : IHRSettingsService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        private readonly AppSettings? _settings;

        public HRSettingsService(HumanResourcesDBContext ctx, IHRLogService log,
                                 IOptions<AppSettings> opts)
        {
            _ctx = ctx; _log = log; _settings = opts.Value;
            // EPPlus 7+ free-license setting — required at app start.
            ExcelPackage.License.SetNonCommercialPersonal("SBERP HR");
        }

        public async Task<DataResponse> GetHRSettingsAsync()
        {
            try
            {
                var s = await _ctx.HRSettings!
                    .AsNoTracking()
                    .Where(x => x.IsActive == true)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync();

                if (s == null)
                    return Utilities.Warn(ConstantSupplier.HR_SETTINGS_NOT_FOUND,
                                          code: (int)HttpStatusCode.NotFound);

                var dto = new HRSettingsResponse
                {
                    Id = s.Id,
                    AttendanceSource = s.AttendanceSource,
                    AttendanceSourceName = s.AttendanceSource == 1
                        ? "Manual Excel Upload" : "Biometric",
                    BiometricProvider = s.BiometricProvider,
                    BiometricProviderName = s.BiometricProvider switch
                    {
                        1 => "Fingerprint", 2 => "Face",
                        3 => "Iris",        4 => "Multi", _ => "None"
                    },
                    BiometricConnectionString = s.BiometricConnectionString,
                    BiometricSourceObject = s.BiometricSourceObject,
                    OfficeStartTime = s.OfficeStartTime,
                    OfficeEndTime = s.OfficeEndTime,
                    GracePeriodMinutes = s.GracePeriodMinutes,
                    WeeklyOffDays = s.WeeklyOffDays,
                    AnnualLeaveDays = s.AnnualLeaveDays,
                    SickLeaveDays = s.SickLeaveDays,
                    CasualLeaveDays = s.CasualLeaveDays,
                    AutoProcessTime = s.AutoProcessTime,
                    AutoProcessEnabled = s.AutoProcessEnabled,
                    IsActive = s.IsActive
                };

                return Utilities.Ok(ConstantSupplier.HR_SETTINGS_FETCH_SUCCESS, dto);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetHRSettingsAsync)); }
        }

        public async Task<DataResponse> SaveUpdateHRSettingsAsync(HRSettingsRequest r)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                HRSettings? s = null;
                if (Guid.TryParse(r.Id, out var g))
                    s = await _ctx.HRSettings!.FirstOrDefaultAsync(x => x.Id == g);

                if (s == null)
                {
                    s = await _ctx.HRSettings!
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefaultAsync();
                }

                bool isCreate = s == null;
                if (isCreate)
                {
                    s = new HRSettings { Id = Guid.NewGuid(),
                                         CreatedBy = r.CreateUpdateBy,
                                         CreatedDate = DateTime.UtcNow };
                }

                s.AttendanceSource         = r.AttendanceSource;
                s.BiometricProvider        = r.BiometricProvider;
                s.BiometricConnectionString= r.BiometricConnectionString;
                s.BiometricSourceObject    = r.BiometricSourceObject;
                s.OfficeStartTime          = r.OfficeStartTime;
                s.OfficeEndTime            = r.OfficeEndTime;
                s.GracePeriodMinutes       = r.GracePeriodMinutes;
                s.WeeklyOffDays            = r.WeeklyOffDays;
                s.AnnualLeaveDays          = r.AnnualLeaveDays;
                s.SickLeaveDays            = r.SickLeaveDays;
                s.CasualLeaveDays          = r.CasualLeaveDays;
                s.AutoProcessTime          = r.AutoProcessTime;
                s.AutoProcessEnabled       = r.AutoProcessEnabled;
                s.IsActive                 = r.IsActive ?? true;

                if (!isCreate)
                {
                    s.UpdatedBy   = r.CreateUpdateBy;
                    s.UpdatedDate = DateTime.UtcNow;
                }

                if (isCreate) await _ctx.HRSettings!.AddAsync(s);
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Utilities.Ok(ConstantSupplier.HR_SETTINGS_SAVE_SUCCESS, s.Id);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(SaveUpdateHRSettingsAsync));
            }
        }

        // =================================================================
        // Excel upload — parses, validates, and inserts attendance rows
        // =================================================================
        public async Task<DataResponse> UploadAttendanceExcelAsync(IFormFile file, string uploadedBy)
        {
            if (file == null || file.Length == 0)
                return Utilities.Warn(ConstantSupplier.ATTENDANCE_EXCEL_INVALID);

            // Save the file to disk so HR can re-download for audit.
            var uploadDir = _settings?.Attendance?.UploadRootPath
                            ?? Path.Combine(AppContext.BaseDirectory, "uploads", "attendance");
            Directory.CreateDirectory(uploadDir);
            var savedName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Path.GetFileName(file.FileName)}";
            var savedPath = Path.Combine(uploadDir, savedName);
            using (var fs = File.Create(savedPath))
                await file.CopyToAsync(fs);

            var batchId = Guid.NewGuid();
            var batch = new AttendanceUploadLog
            {
                Id = batchId,
                FileName = file.FileName,
                FilePath = savedPath,
                FileSizeBytes = file.Length,
                UploadedBy = uploadedBy,
                UploadedDate = DateTime.UtcNow,
                IsRolledBack = false
            };

            var rows         = new List<Attendance>();
            var errorRows    = new List<object>();
            int totalRows    = 0;
            int successRows  = 0;
            int failedRows   = 0;

            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                // Cache employees by code for fast lookup
                var empByCode = await _ctx.Employees!
                    .Where(e => e.IsActive == true)
                    .ToDictionaryAsync(e => e.EmployeeCode!, e => e.Id);

                using var stream = new FileStream(savedPath, FileMode.Open, FileAccess.Read);
                using var pkg = new ExcelPackage(stream);
                var ws = pkg.Workbook.Worksheets.FirstOrDefault();
                if (ws == null || ws.Dimension == null)
                    return Utilities.Warn(ConstantSupplier.ATTENDANCE_EXCEL_INVALID);

                // Expected header in row 1: EmployeeCode | Date | CheckIn | CheckOut | Status
                int rowStart = 2, rowEnd = ws.Dimension.End.Row;

                for (int row = rowStart; row <= rowEnd; row++)
                {
                    totalRows++;
                    var codeCell   = ws.Cells[row, 1].Text?.Trim();
                    var dateCell   = ws.Cells[row, 2].Text?.Trim();
                    var inCell     = ws.Cells[row, 3].Text?.Trim();
                    var outCell    = ws.Cells[row, 4].Text?.Trim();
                    var statusCell = ws.Cells[row, 5].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(codeCell) && string.IsNullOrWhiteSpace(dateCell))
                        continue;   // blank line, skip silently

                    if (!empByCode.TryGetValue(codeCell ?? "", out var empId))
                    {
                        failedRows++;
                        errorRows.Add(new { Row = row, Reason = "Unknown EmployeeCode", EmployeeCode = codeCell });
                        continue;
                    }

                    if (!DateTime.TryParse(dateCell, out var attDate))
                    {
                        failedRows++;
                        errorRows.Add(new { Row = row, Reason = "Invalid Date", Date = dateCell });
                        continue;
                    }

                    TimeSpan? checkIn = TimeSpan.TryParse(inCell, out var ti) ? ti : null;
                    TimeSpan? checkOut = TimeSpan.TryParse(outCell, out var to) ? to : null;

                    int status = ParseStatus(statusCell)
                                 ?? (checkIn.HasValue
                                     ? (int)EnumAttendanceStatus.Present
                                     : (int)EnumAttendanceStatus.Absent);

                    decimal? hours = (checkIn.HasValue && checkOut.HasValue)
                        ? (decimal)(checkOut.Value - checkIn.Value).TotalHours
                        : null;

                    rows.Add(new Attendance
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = empId,
                        AttendanceDate = attDate.Date,
                        CheckInTime = checkIn,
                        CheckOutTime = checkOut,
                        WorkingHours = hours,
                        Status = status,
                        SourceType = (int)EnumAttendanceSource.ManualExcelUpload,
                        UploadBatchId = batchId,
                        CreatedBy = uploadedBy,
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    });
                    successRows++;
                }

                batch.TotalRows = totalRows;
                batch.SuccessRows = successRows;
                batch.FailedRows = failedRows;
                batch.ErrorReport = errorRows.Count > 0
                    ? JsonConvert.SerializeObject(errorRows)
                    : null;

                await _ctx.AttendanceUploadLogs!.AddAsync(batch);

                if (rows.Count > 0)
                {
                    // Deduplicate against existing (EmployeeId, Date) so re-upload
                    // doesn't crash on the unique index.
                    var dates  = rows.Select(r => r.AttendanceDate).Distinct().ToList();
                    var empIds = rows.Select(r => r.EmployeeId).Distinct().ToList();
                    var existing = await _ctx.Attendances!
                        .Where(a => empIds.Contains(a.EmployeeId) && dates.Contains(a.AttendanceDate))
                        .Select(a => new { a.EmployeeId, a.AttendanceDate })
                        .ToListAsync();

                    var existingSet = new HashSet<(Guid, DateTime)>(
                        existing.Select(x => (x.EmployeeId, x.AttendanceDate)));

                    var fresh = rows.Where(r => !existingSet.Contains((r.EmployeeId, r.AttendanceDate)))
                                    .ToList();
                    await _ctx.Attendances!.AddRangeAsync(fresh);
                    successRows = fresh.Count;
                    batch.SuccessRows = successRows;
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();

                return Utilities.Ok(
                    ConstantSupplier.ATTENDANCE_EXCEL_UPLOAD_OK,
                    new { BatchId = batchId, TotalRows = totalRows,
                          SuccessRows = successRows, FailedRows = failedRows,
                          Errors = errorRows });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return Utilities.Exception(ex, _log, nameof(UploadAttendanceExcelAsync));
            }
        }

        private static int? ParseStatus(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return s.Trim().ToLowerInvariant() switch
            {
                "present"  or "p"  => (int)EnumAttendanceStatus.Present,
                "absent"   or "a"  => (int)EnumAttendanceStatus.Absent,
                "leave"    or "l"  => (int)EnumAttendanceStatus.Leave,
                "holiday"  or "h"  => (int)EnumAttendanceStatus.Holiday,
                "weekoff"  or "wo" => (int)EnumAttendanceStatus.WeekOff,
                "halfday"  or "hd" => (int)EnumAttendanceStatus.HalfDay,
                "late"            => (int)EnumAttendanceStatus.Late,
                _ => null
            };
        }
    }
}

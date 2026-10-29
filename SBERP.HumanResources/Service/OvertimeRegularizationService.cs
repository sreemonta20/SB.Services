using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Helper;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Request;
using SBERP.HumanResources.Models.Response;
using SBERP.HumanResources.Persistence;
using System.Data.Common;
using System.Net;

namespace SBERP.HumanResources.Service
{
    /// <summary>Small ADO helper shared by the attendance services for SP reads
    /// returning a single JSON string (same pattern the recruitment services use).</summary>
    internal static class AttendanceAdo
    {
        public static async Task<string?> ScalarSpAsync(HumanResourcesDBContext ctx, string sp, params (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = ps.Length == 0
                ? $"EXEC dbo.{sp}"
                : $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            return (await cmd.ExecuteScalarAsync()) as string;
        }

        public static async Task<(int rows, string msg)> StatusSpAsync(HumanResourcesDBContext ctx, string sp, params (string name, object? value)[] ps)
        {
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open) await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"EXEC dbo.{sp} " + string.Join(", ", ps.Select((p, i) => $"{p.name}=@p{i}"));
            for (int i = 0; i < ps.Length; i++) AddParam(cmd, $"@p{i}", ps[i].value);
            using var reader = await cmd.ExecuteReaderAsync();
            int rows = 0; string msg = ConstantSupplier.FAILED_MSG;
            if (await reader.ReadAsync())
            {
                rows = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
                msg = reader.GetString(reader.GetOrdinal("Message"));
            }
            await reader.CloseAsync();
            return (rows, msg);
        }

        public static void AddParam(DbCommand cmd, string name, object? value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value ?? DBNull.Value; cmd.Parameters.Add(p); }
    }

    public class OvertimeService : IOvertimeService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public OvertimeService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<OvertimeResponse>?> GetAllOvertimePagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<OvertimeResponse>(_ctx, ConstantSupplier.SP_GET_ALL_OVERTIME_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllOvertimePagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateOvertimeAsync(OvertimeRequestRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                if (!ShiftService.TryTime(request.FromTime, out var from) || !ShiftService.TryTime(request.ToTime, out var to))
                    return Utilities.Warn(ConstantSupplier.ATT_TIME_PARSE_ERROR);
                decimal hours = (decimal)Math.Round((to - from).TotalHours, 2);
                if (hours <= 0) return Utilities.Warn("To-time must be after from-time.");

                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid request id");
                    var ot = await _ctx.OvertimeRequests!.FirstOrDefaultAsync(o => o.Id == id);
                    if (ot == null) return Utilities.Warn("Overtime request not found", code: (int)HttpStatusCode.NotFound);
                    if (ot.StatusId != (int)EnumOvertimeStatus.Pending) return Utilities.Warn("Only pending requests can be edited.");
                    ot.EmployeeId = empId; ot.OvertimeDate = request.OvertimeDate;
                    ot.FromTime = from; ot.ToTime = to; ot.Hours = hours; ot.Reason = request.Reason;
                    ot.UpdatedBy = request.CreateUpdateBy; ot.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    int max = await NextCodeSeed("OT");
                    await _ctx.OvertimeRequests!.AddAsync(new OvertimeRequest
                    {
                        Id = Guid.NewGuid(), RequestCode = $"OT-{(max + 1):D4}", EmployeeId = empId,
                        OvertimeDate = request.OvertimeDate, FromTime = from, ToTime = to, Hours = hours,
                        Reason = request.Reason, StatusId = (int)EnumOvertimeStatus.Pending,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateOvertimeAsync)); }
        }

        public async Task<DataResponse> SetOvertimeStatusAsync(AttendanceApprovalRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid request id");
                var (rows, msg) = await AttendanceAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_OVERTIME_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId),
                    ("@ApproverId", Guid.TryParse(request.ApproverId, out var ap) ? ap : (object?)null),
                    ("@ApproverRemarks", request.ApproverRemarks), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetOvertimeStatusAsync)); }
        }

        private async Task<int> NextCodeSeed(string prefix)
        {
            // pull the max numeric suffix already used; SP-side fn is also fine but
            // overtime codes are generated here to keep the insert in EF.
            var codes = await _ctx.OvertimeRequests!
                .Where(o => o.RequestCode!.StartsWith(prefix + "-"))
                .Select(o => o.RequestCode!).ToListAsync();
            int max = 0;
            foreach (var c in codes)
                if (int.TryParse(c.Substring(prefix.Length + 1), out var n) && n > max) max = n;
            return max;
        }
    }

    public class RegularizationService : IRegularizationService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public RegularizationService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<RegularizationResponse>?> GetAllRegularizationsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<RegularizationResponse>(_ctx, ConstantSupplier.SP_GET_ALL_REGULARIZATIONS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllRegularizationsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateRegularizationAsync(RegularizationRequestRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                ShiftService.TryTime(request.RequestedCheckIn, out var inT);
                ShiftService.TryTime(request.RequestedCheckOut, out var outT);
                TimeSpan? reqIn = string.IsNullOrWhiteSpace(request.RequestedCheckIn) ? null : inT;
                TimeSpan? reqOut = string.IsNullOrWhiteSpace(request.RequestedCheckOut) ? null : outT;

                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);
                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid request id");
                    var r = await _ctx.AttendanceRegularizationRequests!.FirstOrDefaultAsync(x => x.Id == id);
                    if (r == null) return Utilities.Warn("Regularization not found", code: (int)HttpStatusCode.NotFound);
                    if (r.StatusId != (int)EnumRegularizationStatus.Pending) return Utilities.Warn("Only pending requests can be edited.");
                    r.AttendanceDate = request.AttendanceDate; r.RegTypeId = request.RegTypeId;
                    r.RequestedCheckIn = reqIn; r.RequestedCheckOut = reqOut; r.Reason = request.Reason;
                    r.AttendanceId = Guid.TryParse(request.AttendanceId, out var aid) ? aid : null;
                    r.UpdatedBy = request.CreateUpdateBy; r.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    int max = await NextCodeSeed("REG");
                    await _ctx.AttendanceRegularizationRequests!.AddAsync(new AttendanceRegularizationRequest
                    {
                        Id = Guid.NewGuid(), RequestCode = $"REG-{(max + 1):D4}", EmployeeId = empId,
                        AttendanceId = Guid.TryParse(request.AttendanceId, out var aid) ? aid : null,
                        AttendanceDate = request.AttendanceDate, RegTypeId = request.RegTypeId,
                        RequestedCheckIn = reqIn, RequestedCheckOut = reqOut, Reason = request.Reason,
                        StatusId = (int)EnumRegularizationStatus.Pending,
                        CreatedBy = request.CreateUpdateBy, CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }
                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateRegularizationAsync)); }
        }

        public async Task<DataResponse> SetRegularizationStatusAsync(AttendanceApprovalRequest request)
        {
            try
            {
                if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid request id");
                var (rows, msg) = await AttendanceAdo.StatusSpAsync(_ctx, ConstantSupplier.SP_SET_REGULARIZATION_STATUS,
                    ("@Id", id), ("@StatusId", request.StatusId),
                    ("@ApproverId", Guid.TryParse(request.ApproverId, out var ap) ? ap : (object?)null),
                    ("@ApproverRemarks", request.ApproverRemarks), ("@UpdatedBy", request.CreateUpdateBy));
                return rows > 0 ? Utilities.Ok(msg, null) : Utilities.Warn(msg);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(SetRegularizationStatusAsync)); }
        }

        private async Task<int> NextCodeSeed(string prefix)
        {
            var codes = await _ctx.AttendanceRegularizationRequests!
                .Where(o => o.RequestCode!.StartsWith(prefix + "-"))
                .Select(o => o.RequestCode!).ToListAsync();
            int max = 0;
            foreach (var c in codes)
                if (int.TryParse(c.Substring(prefix.Length + 1), out var n) && n > max) max = n;
            return max;
        }
    }

    public class AttendanceSummaryService : IAttendanceSummaryService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public AttendanceSummaryService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> GetMonthlyAttendanceSummaryAsync(DateTime month, string? departmentId, string? employeeId)
        {
            try
            {
                var json = await AttendanceAdo.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_MONTHLY_ATTENDANCE_SUMMARY,
                    ("@Month", month),
                    ("@DepartmentId", Guid.TryParse(departmentId, out var d) ? d : (object?)null),
                    ("@EmployeeId", Guid.TryParse(employeeId, out var e) ? e : (object?)null));
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<MonthlyAttendanceSummaryResponse>()
                    : JsonConvert.DeserializeObject<List<MonthlyAttendanceSummaryResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.SUMMARY_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetMonthlyAttendanceSummaryAsync)); }
        }
    }
}

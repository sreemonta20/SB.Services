using Microsoft.EntityFrameworkCore;
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

namespace SBERP.HumanResources.Service
{
    /// <summary>Shift CRUD. EF for writes, JSON-SP for paging (PagingHelper).</summary>
    public class ShiftService : IShiftService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public ShiftService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<PagingResult<ShiftResponse>?> GetAllShiftsPagingWithSearchAsync(PagingSearchFilter filter)
        {
            try { return await PagingHelper.ExecuteAsync<ShiftResponse>(_ctx, ConstantSupplier.SP_GET_ALL_SHIFTS_PAGING, filter); }
            catch (Exception ex) { Utilities.Exception(ex, _log, nameof(GetAllShiftsPagingWithSearchAsync)); return null; }
        }

        public async Task<DataResponse> CreateUpdateShiftAsync(ShiftRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!TryTime(request.StartTime, out var start) || !TryTime(request.EndTime, out var end))
                    return Utilities.Warn(ConstantSupplier.ATT_TIME_PARSE_ERROR);

                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                // unique ShiftCode guard
                bool dup = await _ctx.Shifts!.AnyAsync(s => s.ShiftCode == request.ShiftCode &&
                    (!isUpdate || s.Id.ToString() != request.Id));
                if (dup) return Utilities.Warn("Shift code already exists.");

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid shift id");
                    var shift = await _ctx.Shifts!.FirstOrDefaultAsync(s => s.Id == id);
                    if (shift == null) return Utilities.Warn("Shift not found", code: (int)HttpStatusCode.NotFound);

                    shift.ShiftCode = request.ShiftCode; shift.Name = request.Name;
                    shift.StartTime = start; shift.EndTime = end;
                    shift.IsNightShift = request.IsNightShift; shift.BreakMinutes = request.BreakMinutes;
                    shift.GraceInMinutes = request.GraceInMinutes; shift.GraceOutMinutes = request.GraceOutMinutes;
                    shift.HalfDayHours = request.HalfDayHours; shift.FullDayHours = request.FullDayHours;
                    shift.WeekOffDays = request.WeekOffDays; shift.Description = request.Description;
                    shift.UpdatedBy = request.CreateUpdateBy; shift.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    var shift = new Shift
                    {
                        Id = Guid.NewGuid(), ShiftCode = request.ShiftCode, Name = request.Name,
                        StartTime = start, EndTime = end, IsNightShift = request.IsNightShift,
                        BreakMinutes = request.BreakMinutes, GraceInMinutes = request.GraceInMinutes,
                        GraceOutMinutes = request.GraceOutMinutes, HalfDayHours = request.HalfDayHours,
                        FullDayHours = request.FullDayHours, WeekOffDays = request.WeekOffDays,
                        Description = request.Description, CreatedBy = request.CreateUpdateBy,
                        CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.Shifts!.AddAsync(shift);
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateShiftAsync)); }
        }

        public async Task<DataResponse> DeleteShiftAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid shift id");
                var shift = await _ctx.Shifts!.FirstOrDefaultAsync(s => s.Id == g);
                if (shift == null) return Utilities.Warn("Shift not found", code: (int)HttpStatusCode.NotFound);

                bool inUse = await _ctx.ShiftRosters!.AnyAsync(r => r.ShiftId == g && r.IsActive == true);
                if (inUse) return Utilities.Warn("Shift is assigned in a roster and can't be removed.");

                shift.IsActive = false; shift.UpdatedDate = DateTime.UtcNow;  // soft delete
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteShiftAsync)); }
        }

        internal static bool TryTime(string? s, out TimeSpan t)
        {
            t = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            return TimeSpan.TryParseExact(s, new[] { @"hh\:mm", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out t)
                || TimeSpan.TryParse(s, out t);
        }
    }

    /// <summary>Roster assignment. EF for writes, JSON-SP for the per-employee read.</summary>
    public class RosterService : IRosterService
    {
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public RosterService(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task<DataResponse> CreateUpdateRosterAsync(ShiftRosterRequest request)
        {
            await using var tx = await _ctx.Database.BeginTransactionAsync();
            try
            {
                if (!Guid.TryParse(request.EmployeeId, out var empId)) return Utilities.Warn("Invalid employee id");
                if (!Guid.TryParse(request.ShiftId, out var shiftId)) return Utilities.Warn("Invalid shift id");
                bool isUpdate = string.Equals(request.ActionName, "Update", StringComparison.OrdinalIgnoreCase);

                if (isUpdate)
                {
                    if (!Guid.TryParse(request.Id, out var id)) return Utilities.Warn("Invalid roster id");
                    var roster = await _ctx.ShiftRosters!.FirstOrDefaultAsync(r => r.Id == id);
                    if (roster == null) return Utilities.Warn("Roster not found", code: (int)HttpStatusCode.NotFound);
                    roster.EmployeeId = empId; roster.ShiftId = shiftId;
                    roster.EffectiveFrom = request.EffectiveFrom; roster.EffectiveTo = request.EffectiveTo;
                    roster.Remarks = request.Remarks;
                    roster.UpdatedBy = request.CreateUpdateBy; roster.UpdatedDate = DateTime.UtcNow;
                }
                else
                {
                    // close any open-ended roster for this employee before the new one starts
                    var open = await _ctx.ShiftRosters!
                        .Where(r => r.EmployeeId == empId && r.IsActive == true && r.EffectiveTo == null)
                        .ToListAsync();
                    foreach (var r in open)
                    {
                        r.EffectiveTo = request.EffectiveFrom.AddDays(-1);
                        r.UpdatedBy = request.CreateUpdateBy; r.UpdatedDate = DateTime.UtcNow;
                    }

                    await _ctx.ShiftRosters!.AddAsync(new ShiftRoster
                    {
                        Id = Guid.NewGuid(), EmployeeId = empId, ShiftId = shiftId,
                        EffectiveFrom = request.EffectiveFrom, EffectiveTo = request.EffectiveTo,
                        Remarks = request.Remarks, CreatedBy = request.CreateUpdateBy,
                        CreatedDate = DateTime.UtcNow, IsActive = true
                    });
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
                return Utilities.Ok(ConstantSupplier.ROSTER_SAVE_SUCCESS, null);
            }
            catch (Exception ex) { await tx.RollbackAsync(); return Utilities.Exception(ex, _log, nameof(CreateUpdateRosterAsync)); }
        }

        public async Task<DataResponse> GetEmployeeRosterAsync(string employeeId)
        {
            try
            {
                if (!Guid.TryParse(employeeId, out var g)) return Utilities.Warn("Invalid employee id");
                var json = await AttendanceAdo.ScalarSpAsync(_ctx, ConstantSupplier.SP_GET_EMPLOYEE_ROSTER, ("@EmployeeId", g));
                var items = string.IsNullOrWhiteSpace(json)
                    ? new List<RosterResponse>()
                    : JsonConvert.DeserializeObject<List<RosterResponse>>(json!) ?? new();
                return Utilities.Ok(ConstantSupplier.ROSTER_FETCH_SUCCESS, items);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(GetEmployeeRosterAsync)); }
        }

        public async Task<DataResponse> DeleteRosterAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var g)) return Utilities.Warn("Invalid roster id");
                var roster = await _ctx.ShiftRosters!.FirstOrDefaultAsync(r => r.Id == g);
                if (roster == null) return Utilities.Warn("Roster not found", code: (int)HttpStatusCode.NotFound);
                roster.IsActive = false; roster.UpdatedDate = DateTime.UtcNow;
                await _ctx.SaveChangesAsync();
                return Utilities.Ok(ConstantSupplier.SUCCESS_MSG, null);
            }
            catch (Exception ex) { return Utilities.Exception(ex, _log, nameof(DeleteRosterAsync)); }
        }
    }
}

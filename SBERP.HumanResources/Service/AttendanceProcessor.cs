using Microsoft.EntityFrameworkCore;
using SBERP.HumanResources.Enum;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Persistence;
using System.Globalization;

namespace SBERP.HumanResources.Service
{
    /// <summary>
    /// Turns accepted punches into the daily dbo.Attendances row (SourceType 2 = Biometric):
    /// earliest punch of the work day = check-in; latest = check-out when it is at least
    /// HRSettings.MinMinutesBetweenInOut after the check-in. Lateness / half day use the employee's
    /// roster shift, else HRSettings.OfficeStartTime + GracePeriodMinutes.
    /// Rows HR entered themselves (Excel upload, approved regularization) are never overwritten.
    /// </summary>
    public class AttendanceProcessor : IAttendanceProcessor
    {
        private const string RegularizedMark = "[Regularized]";
        private readonly HumanResourcesDBContext _ctx;
        private readonly IHRLogService _log;
        public AttendanceProcessor(HumanResourcesDBContext ctx, IHRLogService log) { _ctx = ctx; _log = log; }

        public async Task RecalculateDayAsync(Guid employeeId, DateTime attendanceDate)
        {
            var settings = await _ctx.HRSettings!.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive == true);
            await RecalculateAsync(employeeId, attendanceDate.Date, settings);
        }

        public async Task<int> RecalculateRangeAsync(DateTime fromDate, DateTime toDate, Guid? employeeId)
        {
            var settings = await _ctx.HRSettings!.AsNoTracking().FirstOrDefaultAsync(s => s.IsActive == true);
            DateTime from = fromDate.Date, to = toDate.Date;
            var days = await _ctx.AttendancePunches!.AsNoTracking()
                .Where(p => p.EmployeeId != null && p.StatusId == (int)EnumPunchStatus.Accepted &&
                            p.AttendanceDate >= from && p.AttendanceDate <= to &&
                            (employeeId == null || p.EmployeeId == employeeId))
                .Select(p => new { EmployeeId = p.EmployeeId!.Value, p.AttendanceDate })
                .Distinct().ToListAsync();

            foreach (var d in days)
            {
                await RecalculateAsync(d.EmployeeId, d.AttendanceDate, settings);
                _ctx.ChangeTracker.Clear();
            }
            return days.Count;
        }

        private async Task RecalculateAsync(Guid employeeId, DateTime date, HRSettings? settings)
        {
            var punches = await _ctx.AttendancePunches!.AsNoTracking()
                .Where(p => p.EmployeeId == employeeId && p.AttendanceDate == date && p.StatusId == (int)EnumPunchStatus.Accepted)
                .OrderBy(p => p.PunchLocalTime).Select(p => p.PunchLocalTime).ToListAsync();
            if (punches.Count == 0) return;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                var row = await _ctx.Attendances!.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == date);
                if (row != null && (row.SourceType != (int)EnumAttendanceSource.Biometric || (row.Remarks ?? "").Contains(RegularizedMark)))
                    return;   // HR's own entry wins

                DateTime checkIn = punches[0], last = punches[^1];
                int minGap = settings?.MinMinutesBetweenInOut ?? 30;
                DateTime? checkOut = (last - checkIn).TotalMinutes >= minGap ? last : null;
                decimal? hours = checkOut.HasValue ? Math.Round((decimal)(checkOut.Value - checkIn).TotalHours, 2) : null;
                int status = await StatusAsync(employeeId, date, checkIn, hours, settings);

                if (row == null)
                {
                    row = new Attendance
                    {
                        Id = Guid.NewGuid(), EmployeeId = employeeId, AttendanceDate = date,
                        SourceType = (int)EnumAttendanceSource.Biometric,
                        CreatedBy = employeeId.ToString(), CreatedDate = DateTime.UtcNow, IsActive = true
                    };
                    await _ctx.Attendances!.AddAsync(row);
                }
                else
                {
                    row.UpdatedBy = employeeId.ToString();
                    row.UpdatedDate = DateTime.UtcNow;
                }
                row.CheckInTime = checkIn.TimeOfDay;
                row.CheckOutTime = checkOut?.TimeOfDay;
                row.WorkingHours = hours;
                row.Status = status;
                row.Remarks = $"Biometric: {punches.Count} punch(es)";

                try
                {
                    await _ctx.SaveChangesAsync();
                    return;
                }
                catch (DbUpdateException) when (attempt == 0)
                {
                    // a parallel punch created the row first: reload and update it
                    _ctx.ChangeTracker.Clear();
                }
            }
        }

        private async Task<int> StatusAsync(Guid employeeId, DateTime date, DateTime checkIn, decimal? hours, HRSettings? settings)
        {
            var shift = await _ctx.ShiftRosters!.AsNoTracking()
                .Where(r => r.EmployeeId == employeeId && r.IsActive == true && r.EffectiveFrom <= date &&
                            (r.EffectiveTo == null || r.EffectiveTo >= date))
                .OrderByDescending(r => r.EffectiveFrom)
                .Select(r => r.Shift).FirstOrDefaultAsync();

            TimeSpan start; int grace; decimal? halfDayHours = null;
            if (shift != null)
            {
                start = shift.StartTime; grace = shift.GraceInMinutes; halfDayHours = shift.HalfDayHours;
            }
            else
            {
                start = TimeSpan.TryParse(settings?.OfficeStartTime, CultureInfo.InvariantCulture, out var st) ? st : new TimeSpan(8, 30, 0);
                grace = settings?.GracePeriodMinutes ?? 0;
            }

            // a shift that starts before the day cut-off (e.g. 02:00) starts on the next calendar day
            var cutoff = new AttendanceClock(settings).DayCutoff;
            DateTime scheduledStart = date + start;
            if (start < cutoff) scheduledStart = scheduledStart.AddDays(1);

            if (hours.HasValue && halfDayHours.HasValue && hours.Value < halfDayHours.Value)
                return (int)EnumAttendanceStatus.HalfDay;
            if (checkIn > scheduledStart.AddMinutes(grace))
                return (int)EnumAttendanceStatus.Late;
            return (int)EnumAttendanceStatus.Present;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    // ActionName drives the single createUpdate* endpoints ("Save"/"Update").
    public class ShiftRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? ShiftCode { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public string? StartTime { get; set; }   // "HH:mm" / "HH:mm:ss"
        [Required] public string? EndTime { get; set; }
        public bool IsNightShift { get; set; }
        public int BreakMinutes { get; set; }
        public int GraceInMinutes { get; set; }
        public int GraceOutMinutes { get; set; }
        public decimal? HalfDayHours { get; set; }
        public decimal? FullDayHours { get; set; }
        public string? WeekOffDays { get; set; }
        public string? Description { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class ShiftRosterRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public string? ShiftId { get; set; }
        [Required] public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class OvertimeRequestRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? EmployeeId { get; set; }
        [Required] public DateTime OvertimeDate { get; set; }
        [Required] public string? FromTime { get; set; }
        [Required] public string? ToTime { get; set; }
        public string? Reason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class RegularizationRequestRequest
    {
        public string? ActionName { get; set; }
        public string? Id { get; set; }
        [Required] public string? EmployeeId { get; set; }
        public string? AttendanceId { get; set; }
        [Required] public DateTime AttendanceDate { get; set; }
        [Required] public int RegTypeId { get; set; }
        public string? RequestedCheckIn { get; set; }
        public string? RequestedCheckOut { get; set; }
        public string? Reason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>Approve / reject / cancel an overtime or regularization request.</summary>
    public class AttendanceApprovalRequest
    {
        [Required] public string? Id { get; set; }
        public int StatusId { get; set; }           // 2 Approved / 3 Rejected / 4 Cancelled
        public string? ApproverId { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class ShiftResponse
    {
        public Guid Id { get; set; }
        public string? ShiftCode { get; set; }
        public string? Name { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsNightShift { get; set; }
        public int BreakMinutes { get; set; }
        public int GraceInMinutes { get; set; }
        public int GraceOutMinutes { get; set; }
        public decimal? FullDayHours { get; set; }
        public decimal? HalfDayHours { get; set; }
        public string? WeekOffDays { get; set; }
        public bool? IsActive { get; set; }
    }

    public class RosterResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }
        public string? ShiftCode { get; set; }
        public string? ShiftName { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public bool? IsActive { get; set; }
    }

    public class OvertimeResponse
    {
        public Guid Id { get; set; }
        public string? RequestCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public DateTime? OvertimeDate { get; set; }
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public decimal? Hours { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? ApproverRemarks { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public class RegularizationResponse
    {
        public Guid Id { get; set; }
        public string? RequestCode { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int RegTypeId { get; set; }
        public string? RegTypeName { get; set; }
        public TimeSpan? RequestedCheckIn { get; set; }
        public TimeSpan? RequestedCheckOut { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? ApproverRemarks { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public class MonthlyAttendanceSummaryResponse
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? DepartmentName { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public int HalfDays { get; set; }
        public int LateDays { get; set; }
        public decimal TotalHours { get; set; }
    }
}

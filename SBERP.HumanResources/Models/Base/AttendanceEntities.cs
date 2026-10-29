using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    // Lookups — Ids mirror enums in SBERP.HumanResources.Enum.
    public class OvertimeStatusLookup       : BaseLookup { }
    public class RegularizationStatusLookup : BaseLookup { }
    public class RegularizationTypeLookup   : BaseLookup { }

    /// <summary>A named working window. Drives lateness / half-day logic.</summary>
    public class Shift
    {
        [Key] public Guid Id { get; set; }
        public string? ShiftCode { get; set; }
        public string? Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsNightShift { get; set; }
        public int BreakMinutes { get; set; }
        public int GraceInMinutes { get; set; }
        public int GraceOutMinutes { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? HalfDayHours { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? FullDayHours { get; set; }
        public string? WeekOffDays { get; set; }   // CSV of weekday numbers
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ShiftLog
    {
        [Key] public Guid Id { get; set; }
        public Guid ShiftId { get; set; }
        public string? ShiftCode { get; set; }
        public string? Name { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    /// <summary>Assigns an employee to a shift over a date range.</summary>
    public class ShiftRoster
    {
        [Key] public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual Shift? Shift { get; set; }
    }

    public class ShiftRosterLog
    {
        [Key] public Guid Id { get; set; }
        public Guid ShiftRosterId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? ShiftId { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class OvertimeRequest
    {
        [Key] public Guid Id { get; set; }
        public string? RequestCode { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime OvertimeDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? Hours { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class OvertimeRequestLog
    {
        [Key] public Guid Id { get; set; }
        public Guid OvertimeRequestId { get; set; }
        public string? RequestCode { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? OvertimeDate { get; set; }
        [Column(TypeName = "decimal(5,2)")] public decimal? Hours { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class AttendanceRegularizationRequest
    {
        [Key] public Guid Id { get; set; }
        public string? RequestCode { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid? AttendanceId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int RegTypeId { get; set; }
        public TimeSpan? RequestedCheckIn { get; set; }
        public TimeSpan? RequestedCheckOut { get; set; }
        public string? Reason { get; set; }
        public int StatusId { get; set; }
        public Guid? ApproverId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApproverRemarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class AttendanceRegularizationRequestLog
    {
        [Key] public Guid Id { get; set; }
        public Guid RegularizationRequestId { get; set; }
        public string? RequestCode { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int? RegTypeId { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

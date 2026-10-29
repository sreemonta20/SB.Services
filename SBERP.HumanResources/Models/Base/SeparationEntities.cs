using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SBERP.HumanResources.Models.Base
{
    public class SeparationTypeLookup       : BaseLookup { }
    public class SeparationStatusLookup     : BaseLookup { }
    public class ClearanceStatusLookup      : BaseLookup { }
    public class ClearanceDepartmentLookup  : BaseLookup { }

    public class Separation
    {
        [Key] public Guid Id { get; set; }
        public string? SeparationCode { get; set; }
        public Guid EmployeeId { get; set; }
        public int SeparationTypeId { get; set; }
        public int StatusId { get; set; }
        public string? Reason { get; set; }
        public DateTime? NoticeDate { get; set; }
        public DateTime? RequestedLastWorkingDate { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public int? NoticePeriodDays { get; set; }
        public bool IsNoticeWaived { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool ExitInterviewDone { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string? SettlementRemarks { get; set; }
        public bool? IsRehireEligible { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual ICollection<SeparationClearance>? Clearances { get; set; }
    }

    public class SeparationLog
    {
        [Key] public Guid Id { get; set; }
        public Guid SeparationId { get; set; }
        public string? SeparationCode { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? SeparationTypeId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class SeparationClearance
    {
        [Key] public Guid Id { get; set; }
        public Guid SeparationId { get; set; }
        public int DepartmentId { get; set; }
        public int ClearanceStatusId { get; set; }
        public string? Remarks { get; set; }
        public Guid? ClearedBy { get; set; }
        public DateTime? ClearedDate { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal? DuesAmount { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class SeparationClearanceLog
    {
        [Key] public Guid Id { get; set; }
        public Guid SeparationClearanceId { get; set; }
        public Guid? SeparationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? ClearanceStatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class ExitInterview
    {
        [Key] public Guid Id { get; set; }
        public Guid SeparationId { get; set; }
        public DateTime? InterviewDate { get; set; }
        public Guid? InterviewerId { get; set; }
        public string? PrimaryReason { get; set; }
        public int? OverallExperienceRating { get; set; }
        public bool? WouldRecommend { get; set; }
        public string? Feedback { get; set; }
        public string? Suggestions { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ExitInterviewLog
    {
        [Key] public Guid Id { get; set; }
        public Guid ExitInterviewId { get; set; }
        public Guid? SeparationId { get; set; }
        public int? OverallExperienceRating { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

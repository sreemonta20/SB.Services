using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    // =========================================================================
    // Recruitment request payloads. ActionName ("Save"/"Update") drives the
    // create-vs-update branch in the service, identical to DepartmentRequest /
    // EmployeeRequest. Workflow transitions (Submit/Approve/Reject/Publish/etc.)
    // use the small *ActionRequest types.
    // =========================================================================

    public class PositionRequest
    {
        public string? ActionName { get; set; }   // Save / Update
        public string? Id { get; set; }
        [Required] public string? Title { get; set; }
        public string? DesignationId { get; set; }
        public string? DepartmentId { get; set; }
        public string? CompanyId { get; set; }
        public string? BranchId { get; set; }
        public string? CostCenterId { get; set; }
        public string? JobGradeId { get; set; }
        public string? ReportsToPositionId { get; set; }
        public int? Grade { get; set; }
        [Range(1, 10000)] public int BudgetedHeadcount { get; set; } = 1;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Description { get; set; }
        public int? EmploymentType { get; set; }
        public string? WorkLocation { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }

    public class RequisitionRequest
    {
        public string? ActionName { get; set; }   // Save / Update
        public string? Id { get; set; }
        [Required] public string? PositionId { get; set; }
        public string? DepartmentId { get; set; }
        public string? DesignationId { get; set; }
        public int NumberOfVacancies { get; set; } = 1;
        public string? RequestedById { get; set; }
        public string? Justification { get; set; }
        public bool? IsReplacement { get; set; }
        public string? ReplacingEmployeeId { get; set; }
        public DateTime? RequiredByDate { get; set; }
        public int? MinExperienceYears { get; set; }
        public string? RequiredQualification { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>Submit / Approve / Reject a requisition.</summary>
    public class RequisitionActionRequest
    {
        [Required] public string? Id { get; set; }
        public string? ApprovedById { get; set; }
        public string? ApprovalRemark { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class JobPostingRequest
    {
        public string? ActionName { get; set; }   // Save / Update
        public string? Id { get; set; }
        [Required] public string? ManpowerRequisitionId { get; set; }
        [Required] public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Responsibilities { get; set; }
        public string? Requirements { get; set; }
        public int? EmploymentType { get; set; }
        public string? WorkLocation { get; set; }
        public bool IsRemote { get; set; }
        public string? SalaryRangeText { get; set; }
        public string? PublishChannels { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string? CreateUpdateBy { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>Public/internal apply form payload (candidate + application in one go).</summary>
    public class CandidateApplicationRequest
    {
        [Required] public string? JobPostingId { get; set; }
        [Required] public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required, EmailAddress] public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentEmployer { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Source { get; set; }
        public string? ReferredByEmployeeId { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>Advance / reject a pipeline card.</summary>
    public class MoveStageRequest
    {
        [Required] public string? Id { get; set; }
        public int StageId { get; set; }
        public decimal? ScreeningScore { get; set; }
        public string? ScreeningNotes { get; set; }
        public string? RejectionReason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class InterviewRequest
    {
        public string? ActionName { get; set; }   // Save / Update / Complete
        public string? Id { get; set; }
        [Required] public string? JobApplicationId { get; set; }
        public int RoundNumber { get; set; } = 1;
        public string? RoundName { get; set; }
        public int? InterviewModeId { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Location { get; set; }
        public string? MeetingLink { get; set; }
        // completion fields
        public decimal? OverallRating { get; set; }
        public int? Recommendation { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Feedback { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class OfferRequest
    {
        public string? ActionName { get; set; }   // Save / Update
        public string? Id { get; set; }
        [Required] public string? JobApplicationId { get; set; }
        [Required] public string? CandidateId { get; set; }
        public string? PositionId { get; set; }
        public string? DesignationId { get; set; }
        public string? DepartmentId { get; set; }
        public decimal? OfferedSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? OfferValidTill { get; set; }
        public string? OfferLetterUrl { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>Issue / Accept / Decline an offer.</summary>
    public class OfferActionRequest
    {
        [Required] public string? Id { get; set; }
        public string? OfferLetterUrl { get; set; }
        public string? DeclineReason { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    // =========================================================================
    // Recruitment lookups — Ids mirror the C# enums in SBERP.HumanResources.Enum.
    // BaseLookup is the existing shared lookup base (Id/Code/Name/SortOrder/IsActive).
    // =========================================================================
    public class RequisitionStatusLookup : BaseLookup { }
    public class JobPostingStatusLookup  : BaseLookup { }
    public class ApplicationStageLookup  : BaseLookup { }
    public class InterviewModeLookup     : BaseLookup { }
    public class OfferStatusLookup       : BaseLookup { }
}

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// A budgeted seat in the org (Module 0 foundation, consumed by Recruitment,
    /// Onboarding and Separation). Distinct from Designation (job title): a
    /// Position is what recruitment hires against, with a headcount.
    /// FilledHeadcount is maintained by SP_SetPositionFill (+1 hire / -1 exit).
    /// </summary>
    public class Position
    {
        [Key] public Guid Id { get; set; }
        public string? PositionCode { get; set; }
        public string? Title { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? CostCenterId { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid? ReportsToPositionId { get; set; }
        public int? Grade { get; set; }
        public int BudgetedHeadcount { get; set; }
        public int FilledHeadcount { get; set; }
        public int StatusId { get; set; } = 2;               // EnumPositionStatus.Open
        public int? EmploymentType { get; set; }
        public string? WorkLocation { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Designation? Designation { get; set; }
    }

    public class PositionLog
    {
        [Key] public Guid Id { get; set; }
        public Guid PositionId { get; set; }
        public string? PositionCode { get; set; }
        public string? Title { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public int? BudgetedHeadcount { get; set; }
        public int? FilledHeadcount { get; set; }
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

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>The recruitment order — request to hire against a Position.</summary>
    public class ManpowerRequisition
    {
        [Key] public Guid Id { get; set; }
        public string? RequisitionCode { get; set; }
        public Guid PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public int NumberOfVacancies { get; set; }
        public Guid? RequestedById { get; set; }
        public string? Justification { get; set; }
        public bool? IsReplacement { get; set; }
        public Guid? ReplacingEmployeeId { get; set; }
        public DateTime? RequiredByDate { get; set; }
        public int? MinExperienceYears { get; set; }
        public string? RequiredQualification { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public int StatusId { get; set; }
        public Guid? ApprovedById { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalRemark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Position? Position { get; set; }
    }

    public class ManpowerRequisitionLog
    {
        [Key] public Guid Id { get; set; }
        public Guid ManpowerRequisitionId { get; set; }
        public string? RequisitionCode { get; set; }
        public Guid? PositionId { get; set; }
        public int? NumberOfVacancies { get; set; }
        public int? StatusId { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovalRemark { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Base
{
    public class JobPosting
    {
        [Key] public Guid Id { get; set; }
        public string? PostingCode { get; set; }
        public Guid ManpowerRequisitionId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Responsibilities { get; set; }
        public string? Requirements { get; set; }
        public int? EmploymentType { get; set; }
        public string? WorkLocation { get; set; }
        public bool? IsRemote { get; set; }
        public string? SalaryRangeText { get; set; }
        public string? PublishChannels { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ManpowerRequisition? ManpowerRequisition { get; set; }
    }

    public class JobPostingLog
    {
        [Key] public Guid Id { get; set; }
        public Guid JobPostingId { get; set; }
        public string? PostingCode { get; set; }
        public string? Title { get; set; }
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

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>Applicant master — kept separate from Employee until hired.</summary>
    public class Candidate
    {
        [Key] public Guid Id { get; set; }
        public string? CandidateCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? CurrentLocation { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentEmployer { get; set; }
        public string? CurrentDesignation { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public int? NoticePeriodDays { get; set; }
        public string? HighestQualification { get; set; }
        public string? ResumeUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Source { get; set; }
        public Guid? ReferredByEmployeeId { get; set; }
        public bool? IsBlacklisted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CandidateLog
    {
        [Key] public Guid Id { get; set; }
        public Guid CandidateId { get; set; }
        public string? CandidateCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool? IsBlacklisted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Base
{
    public class JobApplication
    {
        [Key] public Guid Id { get; set; }
        public string? ApplicationCode { get; set; }
        public Guid JobPostingId { get; set; }
        public Guid CandidateId { get; set; }
        public int StageId { get; set; }
        public DateTime? AppliedDate { get; set; }
        public decimal? ScreeningScore { get; set; }
        public string? ScreeningNotes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? RejectedDate { get; set; }
        public Guid? AssignedRecruiterId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual JobPosting? JobPosting { get; set; }
        public virtual Candidate? Candidate { get; set; }
    }

    public class JobApplicationLog
    {
        [Key] public Guid Id { get; set; }
        public Guid JobApplicationId { get; set; }
        public string? ApplicationCode { get; set; }
        public int? StageId { get; set; }
        public string? RejectionReason { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Base
{
    public class Interview
    {
        [Key] public Guid Id { get; set; }
        public Guid JobApplicationId { get; set; }
        public int RoundNumber { get; set; }
        public string? RoundName { get; set; }
        public int? InterviewModeId { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Location { get; set; }
        public string? MeetingLink { get; set; }
        public decimal? OverallRating { get; set; }
        public int? Recommendation { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Feedback { get; set; }
        public bool? IsCompleted { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<InterviewPanelMember>? PanelMembers { get; set; }
    }

    public class InterviewPanelMember
    {
        [Key] public Guid Id { get; set; }
        public Guid InterviewId { get; set; }
        public Guid InterviewerEmployeeId { get; set; }
        public bool? IsPanelLead { get; set; }
        public decimal? IndividualRating { get; set; }
        public string? IndividualNotes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Base
{
    public class JobOffer
    {
        [Key] public Guid Id { get; set; }
        public string? OfferCode { get; set; }
        public Guid JobApplicationId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public decimal? OfferedSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? OfferValidTill { get; set; }
        public string? OfferLetterUrl { get; set; }
        public int StatusId { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? RespondedDate { get; set; }
        public string? DeclineReason { get; set; }
        public Guid? ConvertedEmployeeId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual Candidate? Candidate { get; set; }
        public virtual JobApplication? JobApplication { get; set; }
    }

    public class JobOfferLog
    {
        [Key] public Guid Id { get; set; }
        public Guid JobOfferId { get; set; }
        public string? OfferCode { get; set; }
        public int? StatusId { get; set; }
        public decimal? OfferedSalary { get; set; }
        public Guid? ConvertedEmployeeId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }
}

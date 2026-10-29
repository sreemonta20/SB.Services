namespace SBERP.HumanResources.Models.Response
{
    // =========================================================================
    // Recruitment response DTOs. List shapes mirror the columns the paging SPs
    // emit (so PagingResult<T> deserializes cleanly). PagingResult<T> and
    // DataResponse already exist in this namespace (shared).
    // =========================================================================

    public class PositionResponse
    {
        public Guid Id { get; set; }
        public string? PositionCode { get; set; }
        public string? Title { get; set; }
        public int? Grade { get; set; }
        public int BudgetedHeadcount { get; set; }
        public int FilledHeadcount { get; set; }
        public int OpenHeadcount { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? DesignationId { get; set; }
        public string? DesignationName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public Guid? JobGradeId { get; set; }
        public string? GradeName { get; set; }
        public Guid? ReportsToPositionId { get; set; }
        public string? ReportsToTitle { get; set; }
        public int? EmploymentType { get; set; }
        public string? WorkLocation { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? SalaryCurrency { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class RequisitionResponse
    {
        public Guid Id { get; set; }
        public string? RequisitionCode { get; set; }
        public int NumberOfVacancies { get; set; }
        public DateTime? RequiredByDate { get; set; }
        public bool? IsReplacement { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? PositionTitle { get; set; }
        public Guid? PositionId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
        public int PostingCount { get; set; }
        public string? Justification { get; set; }
        public string? ApprovalRemark { get; set; }
    }

    public class JobPostingResponse
    {
        public Guid Id { get; set; }
        public string? PostingCode { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Responsibilities { get; set; }
        public string? Requirements { get; set; }
        public string? WorkLocation { get; set; }
        public bool? IsRemote { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int ApplicantCount { get; set; }
        public Guid? ManpowerRequisitionId { get; set; }
    }

    public class CandidateResponse
    {
        public Guid Id { get; set; }
        public string? CandidateCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public string? CurrentEmployer { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? Source { get; set; }
        public bool? IsBlacklisted { get; set; }
        public string? ResumeUrl { get; set; }
        public int ApplicationCount { get; set; }
    }

    public class ApplicationCardResponse
    {
        public Guid Id { get; set; }
        public string? ApplicationCode { get; set; }
        public int StageId { get; set; }
        public string? StageName { get; set; }
        public DateTime? AppliedDate { get; set; }
        public decimal? ScreeningScore { get; set; }
        public Guid CandidateId { get; set; }
        public string? CandidateName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal? TotalExperienceYears { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Source { get; set; }
    }
}

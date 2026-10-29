using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Request
{
    /// <summary>Kick off onboarding from an accepted offer.</summary>
    public class StartOnboardingRequest
    {
        [Required] public string? JobOfferId { get; set; }
        public string? ReportingManagerId { get; set; }
        public string? WorkLocation { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>Update a single checklist task's status.</summary>
    public class UpdateOnboardingTaskRequest
    {
        [Required] public string? Id { get; set; }
        [Required] public string? OnboardingId { get; set; }
        public int StatusId { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    /// <summary>Add an ad-hoc task to a checklist.</summary>
    public class AddOnboardingTaskRequest
    {
        [Required] public string? OnboardingId { get; set; }
        [Required] public string? Title { get; set; }
        public string? Description { get; set; }
        public string? OwnerRole { get; set; }
        public DateTime? DueDate { get; set; }
        public string? CreateUpdateBy { get; set; }
    }

    public class OnboardingActionRequest
    {
        [Required] public string? Id { get; set; }
        public string? Remarks { get; set; }
        public string? CreateUpdateBy { get; set; }
    }
}

namespace SBERP.HumanResources.Models.Response
{
    public class OnboardingResponse
    {
        public Guid Id { get; set; }
        public string? OnboardingCode { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public int ChecklistTotal { get; set; }
        public int ChecklistDone { get; set; }
        public string? CandidateName { get; set; }
        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
    }

    public class OnboardingTaskResponse
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? OwnerRole { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? SortOrder { get; set; }
    }

    public class AcceptedOfferResponse
    {
        public Guid Id { get; set; }
        public string? OfferCode { get; set; }
        public DateTime? JoiningDate { get; set; }
        public decimal? OfferedSalary { get; set; }
        public string? CandidateName { get; set; }
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public string? DesignationName { get; set; }
    }
}

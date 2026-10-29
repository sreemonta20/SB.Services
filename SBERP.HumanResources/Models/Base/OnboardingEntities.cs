using System.ComponentModel.DataAnnotations;

namespace SBERP.HumanResources.Models.Base
{
    // Lookups — Ids mirror enums in SBERP.HumanResources.Enum.
    public class OnboardingStatusLookup     : BaseLookup { }
    public class OnboardingTaskStatusLookup : BaseLookup { }
}

namespace SBERP.HumanResources.Models.Base
{
    /// <summary>
    /// One onboarding per accepted offer. EmployeeId is set the moment the
    /// Employee row is created from the offer; AppUserId on that Employee stays
    /// NULL until Security consumes the EmployeeOnboardedEvent.
    /// </summary>
    public class Onboarding
    {
        [Key] public Guid Id { get; set; }
        public string? OnboardingCode { get; set; }
        public Guid JobOfferId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? ReportingManagerId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int StatusId { get; set; }
        public int ChecklistTotal { get; set; }
        public int ChecklistDone { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<OnboardingTask>? Tasks { get; set; }
    }

    public class OnboardingLog
    {
        [Key] public Guid Id { get; set; }
        public Guid OnboardingId { get; set; }
        public string? OnboardingCode { get; set; }
        public Guid? JobOfferId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? PerformedUser { get; set; }
        public string? Action { get; set; }
    }

    public class OnboardingTask
    {
        [Key] public Guid Id { get; set; }
        public Guid OnboardingId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? OwnerRole { get; set; }
        public Guid? AssignedToId { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? SortOrder { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Transactional outbox row. Written in the same DB transaction as the
    /// business change; the OutboxPublisher relays it to RabbitMQ and stamps
    /// ProcessedDate. EF-mapped so the service can write rows when needed,
    /// though SP_OnboardEmployeeFromOffer also inserts directly.
    /// </summary>
    public class IntegrationOutbox
    {
        [Key] public Guid Id { get; set; }
        public string? EventType { get; set; }
        public string? RoutingKey { get; set; }
        public string? AggregateType { get; set; }
        public Guid? AggregateId { get; set; }
        public string? Payload { get; set; }
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public int Attempts { get; set; }
        public string? LastError { get; set; }
    }
}

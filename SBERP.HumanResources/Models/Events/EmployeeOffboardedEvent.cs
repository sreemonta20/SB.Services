namespace SBERP.HumanResources.Models.Events
{
    /// <summary>
    /// Integration event published when a separation is settled. This is the
    /// cross-service contract: Security consumes it to deactivate the AppUser
    /// (and optionally revoke sessions / send a goodbye notification). Keep this
    /// shape in sync with the payload built by SP_SettleSeparation and the
    /// consumer on the Security side.
    ///
    /// Mirrors the EmployeeOnboardedEvent pattern and rides the same durable
    /// topic exchange ("sberp.events") via the existing OutboxPublisher — only
    /// the routing key differs. Recommended: lift both events into a shared
    /// SBERP.Contracts assembly referenced by HR and Security.
    /// </summary>
    public class EmployeeOffboardedEvent
    {
        public Guid EmployeeId { get; set; }
        public Guid? AppUserId { get; set; }          // the Security AppUser to deactivate
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public Guid SeparationId { get; set; }
        public string? SeparationCode { get; set; }
        public int SeparationTypeId { get; set; }
        public DateTime? LastWorkingDate { get; set; }

        /// <summary>Routing key the publisher uses on the topic exchange.</summary>
        public const string RoutingKey = "hr.employee.offboarded";
        public const string EventType  = "EmployeeOffboardedEvent";
    }
}

namespace SBERP.HumanResources.Models.Events
{
    /// <summary>
    /// Integration event published when an accepted offer becomes an Employee.
    /// This is the cross-service contract: Security consumes it to create the
    /// AppUser, send the welcome email, and write AppUserId back onto the
    /// Employee. Keep this shape in sync with the payload built by
    /// SP_OnboardEmployeeFromOffer and the consumer on the Security side.
    ///
    /// Recommended: lift this type into a shared contracts assembly
    /// (e.g. SBERP.Contracts) referenced by both HR and Security so the two
    /// services serialize/deserialize against the identical definition.
    /// </summary>
    public class EmployeeOnboardedEvent
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        /// <summary>Designation name; Security stores it as the new login's AppUserProfiles.JobTitle.</summary>
        public string? DesignationName { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public Guid OnboardingId { get; set; }

        /// <summary>Routing key the publisher uses on the topic exchange.</summary>
        public const string RoutingKey = "hr.employee.onboarded";
        public const string EventType  = "EmployeeOnboardedEvent";
    }
}

namespace SBERP.HumanResources.Models.Events
{
    /// <summary>
    /// Security → HR (routing key security.appuser.provisioned): the login for an
    /// onboarded employee exists. Mirrors SBERP.Security.Messaging.AppUserProvisionedEvent.
    /// </summary>
    public class AppUserProvisionedEvent
    {
        public Guid EmployeeId { get; set; }
        public Guid AppUserId { get; set; }
        public Guid AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public DateTime ProvisionedOn { get; set; }
    }
}

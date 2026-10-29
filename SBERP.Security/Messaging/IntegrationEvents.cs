namespace SBERP.Security.Messaging
{
    // =========================================================================
    // Cross-service contracts on the "sberp.events" topic exchange.
    // EmployeeOnboardedEvent mirrors SBERP.HumanResources.Models.Events
    // .EmployeeOnboardedEvent (the JSON written by SP_OnboardEmployeeFromOffer
    // into HR's IntegrationOutbox). Keep both copies in sync — or move them to a
    // shared contracts assembly when a third consumer appears.
    // =========================================================================

    /// <summary>HR → Security: a candidate became an employee (routing key hr.employee.onboarded).</summary>
    public class EmployeeOnboardedEvent
    {
        public Guid EmployeeId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        /// <summary>Designation name at onboarding; copied to AppUserProfiles.JobTitle (null from older publishers).</summary>
        public string? DesignationName { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public Guid OnboardingId { get; set; }
    }

    /// <summary>
    /// HR → Security: a separation was settled (routing key hr.employee.offboarded).
    /// Mirrors SBERP.HumanResources.Models.Events.EmployeeOffboardedEvent, i.e. the
    /// JSON SP_SettleSeparation writes into HR's IntegrationOutbox.
    /// </summary>
    public class EmployeeOffboardedEvent
    {
        public Guid EmployeeId { get; set; }
        /// <summary>Employees.AppUserId on the HR side — null if HR never got the write-back.</summary>
        public Guid? AppUserId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? OfficialEmail { get; set; }
        public Guid SeparationId { get; set; }
        public string? SeparationCode { get; set; }
        public int? SeparationTypeId { get; set; }
        public DateTime? LastWorkingDate { get; set; }
    }

    /// <summary>
    /// Security → HR: the login for an employee exists (routing key
    /// security.appuser.provisioned). HR consumes it to set Employees.AppUserId.
    /// </summary>
    public class AppUserProvisionedEvent
    {
        public Guid EmployeeId { get; set; }
        public Guid AppUserId { get; set; }
        public Guid AppUserProfileId { get; set; }
        public string? UserName { get; set; }
        public DateTime ProvisionedOn { get; set; }
    }

    public static class IntegrationRoutingKeys
    {
        public const string EmployeeOnboarded  = "hr.employee.onboarded";
        public const string AppUserProvisioned = "security.appuser.provisioned";
        public const string EmployeeOffboarded = "hr.employee.offboarded";
    }

    /// <summary>appsettings "RabbitMq" section for the Security service.</summary>
    public class RabbitMqSettings
    {
        public bool Enabled { get; set; } = true;
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "sberp.events";
        public string ExchangeType { get; set; } = "topic";
        /// <summary>Durable queue this service owns for onboarding events.</summary>
        public string OnboardedQueue { get; set; } = "security.employee.onboarded";
        /// <summary>Durable queue this service owns for offboarding events.</summary>
        public string OffboardedQueue { get; set; } = "security.employee.offboarded";
        /// <summary>
        /// Direct exchange for dead letters; each queue dead-letters with routing key
        /// "&lt;queue&gt;.dlq" into its own DLQ (messages that failed twice).
        /// </summary>
        public string DeadLetterExchange { get; set; } = "sberp.events.dlx";
        /// <summary>Seconds between reconnect attempts when the broker is down.</summary>
        public int ReconnectDelaySeconds { get; set; } = 15;
    }

    /// <summary>appsettings "Onboarding" section — how a new employee's login is provisioned.</summary>
    public class OnboardingProvisioningSettings
    {
        /// <summary>Role given to new employees (default: the seeded "User" role).</summary>
        public Guid DefaultRoleId { get; set; } = new("10A9E9E7-CB24-4816-9B94-9DB275A40EDD");
        /// <summary>
        /// AppUserProfile id recorded as CreatedBy on provisioned rows. The audit
        /// triggers resolve CreatedBy against AppUserProfiles.Id, so it must be a
        /// GUID of an existing profile (default: the seeded admin profile).
        /// </summary>
        public Guid SystemActorProfileId { get; set; } = new("C047D662-9F0E-4358-B323-15EC3081312C");
        /// <summary>Base URL put in the welcome e-mail.</summary>
        public string LoginUrl { get; set; } = "http://localhost:4200/";
        /// <summary>
        /// When false (default) welcome e-mails are queued in dbo.EmailOutbox but not sent,
        /// so test onboardings never e-mail out through the configured SMTP account.
        /// </summary>
        public bool SendWelcomeEmail { get; set; } = false;
        public int EmailPollSeconds { get; set; } = 30;
        public int EmailMaxAttempts { get; set; } = 5;
    }

    /// <summary>appsettings "Offboarding" section — when a leaver's login is switched off.</summary>
    public class OffboardingSettings
    {
        /// <summary>
        /// Time zone whose midnight ends the last working day (Windows or IANA id).
        /// The login is disabled at 00:00 local time the day AFTER LastWorkingDate.
        /// </summary>
        public string TimeZoneId { get; set; } = "Arabian Standard Time";
        /// <summary>How often the scheduler looks for logins due to be disabled.</summary>
        public int SweepMinutes { get; set; } = 5;
    }
}

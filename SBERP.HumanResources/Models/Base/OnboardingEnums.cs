namespace SBERP.HumanResources.Enum
{
    // Ids mirror seeded lookup rows (OnboardingStatuses, OnboardingTaskStatuses).
    public enum EnumOnboardingStatus
    {
        Initiated = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum EnumOnboardingTaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Done = 3,
        NotApplicable = 4
    }
}

namespace SBERP.HumanResources.Models.Configuration
{
    /// <summary>
    /// RabbitMQ connection + topology. Sits alongside RedisSettings; bind from
    /// appsettings ("RabbitMq" section). The publisher uses a topic exchange so
    /// Security can bind its queue to the hr.employee.* routing pattern.
    /// </summary>
    public class RabbitMqSettings
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "sberp.events";
        public string ExchangeType { get; set; } = "topic";
        /// <summary>Outbox publisher poll interval, seconds.</summary>
        public int PublishIntervalSeconds { get; set; } = 10;
        /// <summary>Max rows relayed per poll.</summary>
        public int BatchSize { get; set; } = 50;
        /// <summary>Give up (and flag) after this many attempts.</summary>
        public int MaxAttempts { get; set; } = 10;
        /// <summary>Durable queue HR owns for Security's AppUserProvisionedEvent.</summary>
        public string ProvisionedQueue { get; set; } = "hr.appuser.provisioned";
        public string DeadLetterExchange { get; set; } = "sberp.events.dlx";
        public string ProvisionedDeadLetterQueue { get; set; } = "hr.appuser.provisioned.dlq";
        public int ReconnectDelaySeconds { get; set; } = 15;
    }
}

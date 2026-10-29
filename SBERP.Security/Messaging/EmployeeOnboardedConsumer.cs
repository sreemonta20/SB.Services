using Microsoft.Extensions.Options;

namespace SBERP.Security.Messaging
{
    /// <summary>
    /// hr.employee.onboarded → <see cref="EmployeeOnboardedHandler"/> (provisions the login).
    /// Queue security.employee.onboarded, dead letters in security.employee.onboarded.dlq.
    /// </summary>
    public sealed class EmployeeOnboardedConsumer : RabbitMqEventConsumer<EmployeeOnboardedEvent>
    {
        public EmployeeOnboardedConsumer(IServiceScopeFactory scopes, IOptions<RabbitMqSettings> opts,
            ILogger<EmployeeOnboardedConsumer> logger) : base(scopes, opts.Value, logger) { }

        protected override string QueueName => Settings.OnboardedQueue;
        protected override string RoutingKey => IntegrationRoutingKeys.EmployeeOnboarded;
        protected override bool IsValid(EmployeeOnboardedEvent evt) => evt.EmployeeId != Guid.Empty;
    }

    /// <summary>
    /// hr.employee.offboarded → <see cref="EmployeeOffboardedHandler"/> (disables the login,
    /// immediately or at the end of the last working day).
    /// Queue security.employee.offboarded, dead letters in security.employee.offboarded.dlq.
    /// </summary>
    public sealed class EmployeeOffboardedConsumer : RabbitMqEventConsumer<EmployeeOffboardedEvent>
    {
        public EmployeeOffboardedConsumer(IServiceScopeFactory scopes, IOptions<RabbitMqSettings> opts,
            ILogger<EmployeeOffboardedConsumer> logger) : base(scopes, opts.Value, logger) { }

        protected override string QueueName => Settings.OffboardedQueue;
        protected override string RoutingKey => IntegrationRoutingKeys.EmployeeOffboarded;
        protected override bool IsValid(EmployeeOffboardedEvent evt) => evt.EmployeeId != Guid.Empty;
    }
}

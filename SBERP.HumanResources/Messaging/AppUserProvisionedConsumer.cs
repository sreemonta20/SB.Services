using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Models.Events;
using SBERP.HumanResources.Persistence;
using System.Text;

namespace SBERP.HumanResources.Messaging
{
    /// <summary>
    /// Closes the onboarding loop: Security publishes AppUserProvisionedEvent
    /// (security.appuser.provisioned) after creating the login for an employee
    /// HR onboarded; this sets Employees.AppUserId (the soft FK to SecurityDB).
    ///
    /// <para>Idempotent (no-op when AppUserId already matches). Manual ack; one
    /// retry, then dead-lettered to hr.appuser.provisioned.dlq. If RabbitMQ is down
    /// at start-up it retries in the background instead of stopping the host.</para>
    /// </summary>
    public sealed class AppUserProvisionedConsumer : BackgroundService
    {
        private const string RoutingKey = "security.appuser.provisioned";

        private readonly IServiceScopeFactory _scopes;
        private readonly RabbitMqSettings _cfg;
        private readonly ILogger<AppUserProvisionedConsumer> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public AppUserProvisionedConsumer(IServiceScopeFactory scopes, IOptions<RabbitMqSettings> opts,
            ILogger<AppUserProvisionedConsumer> logger)
        {
            _scopes = scopes;
            _cfg = opts.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Connect();
                    _logger.LogInformation("Listening for {RoutingKey} on queue {Queue}.", RoutingKey, _cfg.ProvisionedQueue);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("RabbitMQ not reachable ({Message}); retrying in {Delay}s.", ex.Message, _cfg.ReconnectDelaySeconds);
                    CloseQuietly();
                    try { await Task.Delay(TimeSpan.FromSeconds(_cfg.ReconnectDelaySeconds), stoppingToken); }
                    catch (OperationCanceledException) { return; }
                }
            }
        }

        private void Connect()
        {
            var factory = new ConnectionFactory
            {
                HostName = _cfg.Host, Port = _cfg.Port, UserName = _cfg.UserName, Password = _cfg.Password,
                VirtualHost = _cfg.VirtualHost, DispatchConsumersAsync = true, AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            };
            _connection = factory.CreateConnection("SBERP.HumanResources.ProvisionedConsumer");
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_cfg.ExchangeName, _cfg.ExchangeType, durable: true, autoDelete: false);
            // direct DLX + per-queue routing key: dead letters land only in this service's DLQ
            _channel.ExchangeDeclare(_cfg.DeadLetterExchange, ExchangeType.Direct, durable: true, autoDelete: false);
            _channel.QueueDeclare(_cfg.ProvisionedDeadLetterQueue, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(_cfg.ProvisionedDeadLetterQueue, _cfg.DeadLetterExchange, routingKey: _cfg.ProvisionedDeadLetterQueue);
            _channel.QueueDeclare(_cfg.ProvisionedQueue, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    ["x-dead-letter-exchange"] = _cfg.DeadLetterExchange,
                    ["x-dead-letter-routing-key"] = _cfg.ProvisionedDeadLetterQueue,
                });
            _channel.QueueBind(_cfg.ProvisionedQueue, _cfg.ExchangeName, RoutingKey);
            _channel.BasicQos(0, 10, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnReceivedAsync;
            _channel.BasicConsume(_cfg.ProvisionedQueue, autoAck: false, consumer: consumer);
        }

        private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            try
            {
                var evt = JsonConvert.DeserializeObject<AppUserProvisionedEvent>(json);
                if (evt == null || evt.EmployeeId == Guid.Empty || evt.AppUserId == Guid.Empty)
                {
                    _logger.LogError("Unreadable AppUserProvisionedEvent, dead-lettering: {Json}", json);
                    _channel!.BasicNack(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                using var scope = _scopes.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<HumanResourcesDBContext>();
                var emp = await ctx.Employees!.FirstOrDefaultAsync(e => e.Id == evt.EmployeeId);
                if (emp == null)
                {
                    _logger.LogWarning("AppUserProvisionedEvent for unknown employee {EmployeeId}; dead-lettering.", evt.EmployeeId);
                    _channel!.BasicNack(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                if (emp.AppUserId != evt.AppUserId)
                {
                    emp.AppUserId = evt.AppUserId;
                    emp.UpdatedDate = DateTime.UtcNow;
                    await ctx.SaveChangesAsync();
                    _logger.LogInformation("Linked employee {EmployeeCode} to AppUser {AppUserId} ({UserName}).",
                        emp.EmployeeCode, evt.AppUserId, evt.UserName);
                }

                _channel!.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                var requeue = !ea.Redelivered;
                _logger.LogError(ex, "Failed handling AppUserProvisionedEvent ({Action}): {Json}",
                    requeue ? "re-queued" : "dead-lettered", json);
                _channel!.BasicNack(ea.DeliveryTag, false, requeue);
            }
        }

        private void CloseQuietly()
        {
            try { _channel?.Close(); } catch { /* ignore */ }
            try { _connection?.Close(); } catch { /* ignore */ }
            _channel?.Dispose(); _channel = null;
            _connection?.Dispose(); _connection = null;
        }

        public override void Dispose()
        {
            CloseQuietly();
            base.Dispose();
        }
    }
}

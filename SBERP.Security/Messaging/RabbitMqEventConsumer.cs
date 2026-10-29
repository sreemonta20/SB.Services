using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SBERP.Security.Messaging
{
    /// <summary>Scoped business handler for one integration event type.</summary>
    public interface IIntegrationEventHandler<in TEvent>
    {
        Task HandleAsync(TEvent evt);
    }

    /// <summary>
    /// Base for Security's RabbitMQ consumers. Each subclass owns one durable queue
    /// bound to one routing key on the shared topic exchange.
    ///
    /// <para><b>Dead-lettering:</b> the queue dead-letters to the direct exchange
    /// sberp.events.dlx with routing key = its own DLQ name, and only that DLQ is
    /// bound with that key — so a dead letter lands in exactly one DLQ (a fanout
    /// DLX would copy it into every service's DLQ).</para>
    ///
    /// <para><b>Delivery:</b> manual ack; first failure re-queued, a failing
    /// redelivery is dead-lettered. Handlers must be idempotent.</para>
    ///
    /// <para><b>Resilience:</b> if the broker is down at start-up the host keeps
    /// running and the consumer retries; afterwards automatic recovery reconnects.</para>
    /// </summary>
    public abstract class RabbitMqEventConsumer<TEvent> : BackgroundService where TEvent : class
    {
        private readonly IServiceScopeFactory _scopes;
        protected readonly RabbitMqSettings Settings;
        protected readonly ILogger Logger;
        private IConnection? _connection;
        private IModel? _channel;

        protected RabbitMqEventConsumer(IServiceScopeFactory scopes, RabbitMqSettings settings, ILogger logger)
        {
            _scopes = scopes;
            Settings = settings;
            Logger = logger;
        }

        protected abstract string QueueName { get; }
        protected abstract string RoutingKey { get; }
        protected string DeadLetterQueueName => QueueName + ".dlq";

        /// <summary>Reject payloads that deserialised but are unusable (dead-lettered, not retried).</summary>
        protected virtual bool IsValid(TEvent evt) => true;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Connect();
                    Logger.LogInformation("Listening for {RoutingKey} on queue {Queue}.", RoutingKey, QueueName);
                    return;   // the client's dispatcher drives OnReceivedAsync from here
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("RabbitMQ not reachable for {Queue} ({Message}); retrying in {Delay}s.",
                        QueueName, ex.Message, Settings.ReconnectDelaySeconds);
                    CloseQuietly();
                    try { await Task.Delay(TimeSpan.FromSeconds(Settings.ReconnectDelaySeconds), stoppingToken); }
                    catch (OperationCanceledException) { return; }
                }
            }
        }

        private void Connect()
        {
            _connection = RabbitMqTopology.CreateFactory(Settings, asyncConsumers: true)
                .CreateConnection($"SBERP.Security.{GetType().Name}");
            _channel = _connection.CreateModel();

            RabbitMqTopology.DeclareExchange(_channel, Settings);
            _channel.ExchangeDeclare(Settings.DeadLetterExchange, ExchangeType.Direct, durable: true, autoDelete: false);
            _channel.QueueDeclare(DeadLetterQueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(DeadLetterQueueName, Settings.DeadLetterExchange, routingKey: DeadLetterQueueName);

            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    ["x-dead-letter-exchange"] = Settings.DeadLetterExchange,
                    ["x-dead-letter-routing-key"] = DeadLetterQueueName,
                });
            _channel.QueueBind(QueueName, Settings.ExchangeName, RoutingKey);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 5, global: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnReceivedAsync;
            _channel.BasicConsume(QueueName, autoAck: false, consumer: consumer);
        }

        private async Task OnReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            try
            {
                var evt = JsonConvert.DeserializeObject<TEvent>(json);
                if (evt == null || !IsValid(evt))
                {
                    Logger.LogError("Unusable {Event}, dead-lettering: {Json}", typeof(TEvent).Name, json);
                    _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                using var scope = _scopes.CreateScope();
                await scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TEvent>>().HandleAsync(evt);
                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                var requeue = !ea.Redelivered;
                Logger.LogError(ex, "Failed handling {Event} ({Action}): {Json}",
                    typeof(TEvent).Name, requeue ? "re-queued" : "dead-lettered", json);
                _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: requeue);
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

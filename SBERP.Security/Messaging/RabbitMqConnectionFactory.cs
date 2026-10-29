using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace SBERP.Security.Messaging
{
    internal static class RabbitMqTopology
    {
        public static ConnectionFactory CreateFactory(RabbitMqSettings cfg, bool asyncConsumers) => new()
        {
            HostName = cfg.Host,
            Port = cfg.Port,
            UserName = cfg.UserName,
            Password = cfg.Password,
            VirtualHost = cfg.VirtualHost,
            DispatchConsumersAsync = asyncConsumers,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        };

        /// <summary>Same declaration HR uses — idempotent on the broker.</summary>
        public static void DeclareExchange(IModel channel, RabbitMqSettings cfg)
            => channel.ExchangeDeclare(cfg.ExchangeName, cfg.ExchangeType, durable: true, autoDelete: false);
    }

    public interface IIntegrationEventPublisher
    {
        void Publish<T>(string routingKey, T payload);
    }

    /// <summary>
    /// Publishes Security → other-service events (persistent, JSON). One shared
    /// channel guarded by a lock; RabbitMQ channels are not thread-safe.
    /// </summary>
    public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher, IDisposable
    {
        private readonly RabbitMqSettings _cfg;
        private readonly object _gate = new();
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqIntegrationEventPublisher(IOptions<RabbitMqSettings> opts) => _cfg = opts.Value;

        public void Publish<T>(string routingKey, T payload)
        {
            lock (_gate)
            {
                if (_channel is not { IsOpen: true })
                {
                    _connection ??= RabbitMqTopology.CreateFactory(_cfg, asyncConsumers: false)
                        .CreateConnection("SBERP.Security.Publisher");
                    _channel = _connection.CreateModel();
                    RabbitMqTopology.DeclareExchange(_channel, _cfg);
                }

                var props = _channel.CreateBasicProperties();
                props.ContentType = "application/json";
                props.DeliveryMode = 2;   // persistent
                props.Type = typeof(T).Name;
                props.MessageId = Guid.NewGuid().ToString();
                props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
                _channel.BasicPublish(_cfg.ExchangeName, routingKey, mandatory: false, basicProperties: props, body: body);
            }
        }

        public void Dispose()
        {
            try { _channel?.Close(); } catch { /* shutting down */ }
            try { _connection?.Close(); } catch { /* shutting down */ }
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SBERP.HumanResources.Models.Configuration;

namespace SBERP.HumanResources.Messaging
{
    /// <summary>
    /// Thin RabbitMQ publisher. Declares a durable topic exchange once and
    /// publishes persistent messages with a routing key. Security binds its
    /// queue to "hr.employee.*" (or the specific key) on the same exchange.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>Publish a JSON payload under a routing key. Throws on failure
        /// so the OutboxPublisher can record the error and retry later.</summary>
        void Publish(string routingKey, string jsonPayload, string eventType);
    }

    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly RabbitMqSettings _cfg;
        private readonly object _gate = new();
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMqEventPublisher(IOptions<RabbitMqSettings> opts)
        {
            _cfg = opts.Value;
        }

        private void EnsureChannel()
        {
            if (_channel is { IsOpen: true }) return;
            lock (_gate)
            {
                if (_channel is { IsOpen: true }) return;

                var factory = new ConnectionFactory
                {
                    HostName = _cfg.Host,
                    Port = _cfg.Port,
                    UserName = _cfg.UserName,
                    Password = _cfg.Password,
                    VirtualHost = _cfg.VirtualHost,
                    DispatchConsumersAsync = false,
                    AutomaticRecoveryEnabled = true,
                };

                _connection = factory.CreateConnection("SBERP.HumanResources.Publisher");
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(
                    exchange: _cfg.ExchangeName,
                    type: _cfg.ExchangeType,   // topic
                    durable: true,
                    autoDelete: false);
            }
        }

        public void Publish(string routingKey, string jsonPayload, string eventType)
        {
            EnsureChannel();

            var body = Encoding.UTF8.GetBytes(jsonPayload);
            var props = _channel!.CreateBasicProperties();
            props.ContentType = "application/json";
            props.DeliveryMode = 2;                       // persistent
            props.Type = eventType;
            props.MessageId = Guid.NewGuid().ToString();
            props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _cfg.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body);
        }

        public void Dispose()
        {
            try { _channel?.Close(); } catch { /* ignore */ }
            try { _connection?.Close(); } catch { /* ignore */ }
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}

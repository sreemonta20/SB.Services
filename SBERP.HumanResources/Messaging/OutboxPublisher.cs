using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SBERP.HumanResources.Models.Base;
using SBERP.HumanResources.Models.Configuration;
using SBERP.HumanResources.Persistence;

namespace SBERP.HumanResources.Messaging
{
    /// <summary>
    /// Background relay for the transactional outbox. On an interval it picks up
    /// unprocessed dbo.IntegrationOutbox rows (oldest first), publishes each to
    /// RabbitMQ, and stamps ProcessedDate. Failures increment Attempts and store
    /// LastError; rows past MaxAttempts are left flagged for inspection rather
    /// than blocking the queue.
    ///
    /// Register: services.AddHostedService&lt;OutboxPublisher&gt;();
    /// The publisher is a singleton-friendly EF consumer — it creates its own
    /// scope per poll so it never holds the scoped DbContext across the loop.
    /// </summary>
    public class OutboxPublisher : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IEventPublisher _publisher;
        private readonly RabbitMqSettings _cfg;
        private readonly ILogger<OutboxPublisher> _logger;

        public OutboxPublisher(
            IServiceProvider services,
            IEventPublisher publisher,
            IOptions<RabbitMqSettings> opts,
            ILogger<OutboxPublisher> logger)
        {
            _services = services;
            _publisher = publisher;
            _cfg = opts.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromSeconds(Math.Max(2, _cfg.PublishIntervalSeconds));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RelayBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // never let the loop die — log and keep polling
                    _logger.LogError(ex, "OutboxPublisher poll failed.");
                }

                try { await Task.Delay(delay, stoppingToken); }
                catch (TaskCanceledException) { break; }
            }
        }

        private async Task RelayBatchAsync(CancellationToken ct)
        {
            using var scope = _services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<HumanResourcesDBContext>();

            var batch = await ctx.IntegrationOutbox!
                .Where(o => o.ProcessedDate == null && o.Attempts < _cfg.MaxAttempts)
                .OrderBy(o => o.OccurredOn)
                .Take(_cfg.BatchSize)
                .ToListAsync(ct);

            if (batch.Count == 0) return;

            foreach (var row in batch)
            {
                if (ct.IsCancellationRequested) break;
                try
                {
                    _publisher.Publish(row.RoutingKey!, row.Payload!, row.EventType!);
                    row.ProcessedDate = DateTime.UtcNow;
                    row.LastError = null;
                }
                catch (Exception ex)
                {
                    row.Attempts += 1;
                    row.LastError = Truncate(ex.Message, 2000);
                    _logger.LogWarning(ex,
                        "Failed to publish outbox row {Id} ({EventType}); attempt {Attempts}.",
                        row.Id, row.EventType, row.Attempts);
                }
            }

            // one save for the whole batch — last-write-wins is fine here
            await ctx.SaveChangesAsync(ct);
        }

        private static string Truncate(string s, int max) =>
            string.IsNullOrEmpty(s) || s.Length <= max ? s : s.Substring(0, max);
    }
}

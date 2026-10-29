using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SBERP.EmailService.Models;
using SBERP.EmailService.Service;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Persistence;

namespace SBERP.Security.Messaging
{
    /// <summary>
    /// Delivers dbo.EmailOutbox rows through the existing IEmailService (FluentEmail
    /// SMTP, AppSettings:EmailConfiguration). Registered only when
    /// Onboarding:SendWelcomeEmail is true. A sent row has its Body cleared, so a
    /// one-time password never stays at rest after delivery; failures are retried
    /// up to EmailMaxAttempts with the last error recorded.
    /// </summary>
    public sealed class EmailOutboxSender : BackgroundService
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly OnboardingProvisioningSettings _cfg;
        private readonly ILogger<EmailOutboxSender> _logger;

        public EmailOutboxSender(IServiceScopeFactory scopes, IOptions<OnboardingProvisioningSettings> cfg,
            ILogger<EmailOutboxSender> logger)
        {
            _scopes = scopes;
            _cfg = cfg.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromSeconds(Math.Max(5, _cfg.EmailPollSeconds));
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await SendBatchAsync(stoppingToken); }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "E-mail outbox poll failed.");
                }
                try { await Task.Delay(delay, stoppingToken); } catch (OperationCanceledException) { return; }
            }
        }

        private async Task SendBatchAsync(CancellationToken ct)
        {
            using var scope = _scopes.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<SecurityDBContext>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var smtp = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>().Value.EmailConfiguration;
            if (smtp == null) { _logger.LogWarning("AppSettings:EmailConfiguration missing; outbox not sent."); return; }

            var batch = await ctx.EmailOutbox!
                .Where(m => m.ProcessedDate == null && m.Attempts < _cfg.EmailMaxAttempts)
                .OrderBy(m => m.CreatedDate).Take(20).ToListAsync(ct);

            foreach (var m in batch)
            {
                m.Attempts++;
                try
                {
                    var result = await email.SendEmailAsync(smtp, new Message
                    {
                        To = m.ToAddress, Name = m.ToName, Subject = m.Subject, Body = m.Body
                    });
                    if (result.Successful)
                    {
                        m.ProcessedDate = DateTime.UtcNow;
                        m.Body = null;          // drop the one-time credentials once delivered
                        m.LastError = null;
                    }
                    else
                    {
                        m.LastError = Truncate(string.Join("; ", result.ErrorMessages ?? new List<string>()));
                    }
                }
                catch (Exception ex)
                {
                    m.LastError = Truncate(ex.Message);
                }
                await ctx.SaveChangesAsync(ct);
            }
        }

        private static string Truncate(string s) => s.Length <= 2000 ? s : s[..2000];
    }
}

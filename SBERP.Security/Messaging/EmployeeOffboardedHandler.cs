using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Persistence;
using SBERP.Security.Service;

namespace SBERP.Security.Messaging
{
    /// <summary>
    /// Switches off a leaver's login. Shared by the event handler (immediate case)
    /// and <see cref="LoginDeactivationScheduler"/> (future last working day).
    /// </summary>
    public interface ILoginDeactivationService
    {
        /// <summary>Disable the login of a due schedule row and mark it processed. Caller saves.</summary>
        Task DisableAsync(LoginDeactivation row);

        /// <summary>Queue a bell notification for every active Admin. Caller saves.</summary>
        Task NotifyAdminsAsync(string title, string message, DateTime now);
    }

    public sealed class LoginDeactivationService : ILoginDeactivationService
    {
        private static readonly Guid AdminRoleId = new("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA");

        private readonly SecurityDBContext _ctx;
        private readonly OnboardingProvisioningSettings _provisioning;
        private readonly ILogger<LoginDeactivationService> _logger;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginDeactivationService(SecurityDBContext ctx, IOptions<OnboardingProvisioningSettings> provisioning,
            ILogger<LoginDeactivationService> logger, IRefreshTokenService refreshTokenService)
        {
            _ctx = ctx;
            _refreshTokenService = refreshTokenService;
            _provisioning = provisioning.Value;
            _logger = logger;
        }

        public async Task DisableAsync(LoginDeactivation row)
        {
            var now = DateTime.UtcNow;
            if (row.AppUserId == null)
            {
                // the login may have been provisioned after the schedule was recorded
                var login = await (from p in _ctx.AppUserProfiles!.AsNoTracking()
                                   join u in _ctx.AppUsers!.AsNoTracking() on p.Id equals u.AppUserProfileId
                                   where p.EmployeeId == row.EmployeeId
                                   select new { u.Id, ProfileId = p.Id, u.UserName }).FirstOrDefaultAsync();
                if (login != null)
                {
                    row.AppUserId = login.Id; row.AppUserProfileId = login.ProfileId; row.UserName = login.UserName;
                }
            }
            var user = row.AppUserId == null ? null : await _ctx.AppUsers!.FirstOrDefaultAsync(u => u.Id == row.AppUserId);

            if (user == null)
                row.Outcome = "NoLogin";
            else if (user.IsActive != true)
                row.Outcome = "AlreadyInactive";
            else
            {
                // Login and token refresh both require IsActive; on top of that every session is
                // signed out now: refresh tokens revoked, issued access tokens rejected by all services.
                user.IsActive = false;
                await _refreshTokenService.EndAllSessionsAsync(user.Id, ConstantSupplier.TOKEN_REVOKE_OFFBOARDED);
                user.UpdatedBy = _provisioning.SystemActorProfileId.ToString().ToUpperInvariant();
                user.UpdatedDate = now;
                row.Outcome = "Disabled";
                await NotifyAdminsAsync("Leaver login disabled",
                    $"Login '{row.UserName}' for {row.EmployeeCode} was disabled (separation {row.SeparationCode}).", now);
            }

            row.ProcessedDate = now;
            row.UpdatedDate = now;
            _logger.LogInformation("Login deactivation for {EmployeeCode} ({UserName}), separation {SeparationCode}: {Outcome}.",
                row.EmployeeCode, row.UserName, row.SeparationCode, row.Outcome);
        }

        public async Task NotifyAdminsAsync(string title, string message, DateTime now)
        {
            var adminIds = await _ctx.AppUserProfiles!.AsNoTracking()
                .Where(p => p.AppUserRoleId == AdminRoleId && p.IsActive == true)
                .Select(p => p.Id).ToListAsync();
            foreach (var id in adminIds)
                await _ctx.Notifications!.AddAsync(new Notification
                {
                    Id = Guid.NewGuid(), RecipientProfileId = id, Type = "Offboarding", Title = title,
                    Message = message, RouteLink = "/business/security/appuserprofile", CreatedDate = now
                });
        }
    }

    /// <summary>
    /// Handles HR's EmployeeOffboardedEvent (published when a separation is settled).
    /// <list type="bullet">
    /// <item>Finds the login via AppUserProfiles.EmployeeId (provisioned on onboarding),
    ///   falling back to the event's AppUserId for manually created users linked in HR.</item>
    /// <item>Disables it at 00:00 local time the day after LastWorkingDate (Offboarding:TimeZoneId):
    ///   immediately when that moment has passed, otherwise it's scheduled and the
    ///   scheduler does it on time — a leaver keeps access through their last day.</item>
    /// <item>Idempotent on SeparationId; a redelivery only refreshes a still-pending date.</item>
    /// </list>
    /// </summary>
    public sealed class EmployeeOffboardedHandler : IIntegrationEventHandler<EmployeeOffboardedEvent>
    {
        private readonly SecurityDBContext _ctx;
        private readonly ILoginDeactivationService _deactivation;
        private readonly OffboardingSettings _cfg;
        private readonly ILogger<EmployeeOffboardedHandler> _logger;

        public EmployeeOffboardedHandler(SecurityDBContext ctx, ILoginDeactivationService deactivation,
            IOptions<OffboardingSettings> cfg, ILogger<EmployeeOffboardedHandler> logger)
        {
            _ctx = ctx;
            _deactivation = deactivation;
            _cfg = cfg.Value;
            _logger = logger;
        }

        public async Task HandleAsync(EmployeeOffboardedEvent evt)
        {
            var now = DateTime.UtcNow;
            var deactivateOn = DeactivationInstantUtc(evt.LastWorkingDate, now);

            var row = await _ctx.LoginDeactivations!.FirstOrDefaultAsync(x => x.SeparationId == evt.SeparationId);
            if (row != null)
            {
                if (row.ProcessedDate == null && row.DeactivateOn != deactivateOn)
                {
                    row.DeactivateOn = deactivateOn;
                    row.LastWorkingDate = evt.LastWorkingDate;
                    row.UpdatedDate = now;
                    if (deactivateOn <= now) await _deactivation.DisableAsync(row);
                    await _ctx.SaveChangesAsync();
                }
                _logger.LogInformation("Separation {SeparationCode} already recorded ({State}); nothing more to do.",
                    evt.SeparationCode, row.ProcessedDate == null ? "pending" : row.Outcome);
                return;
            }

            // locate the login: provisioned link first, then HR's AppUserId
            var login = await (from p in _ctx.AppUserProfiles!.AsNoTracking()
                               join u in _ctx.AppUsers!.AsNoTracking() on p.Id equals u.AppUserProfileId
                               where p.EmployeeId == evt.EmployeeId
                               select new { UserId = (Guid?)u.Id, ProfileId = (Guid?)p.Id, u.UserName }).FirstOrDefaultAsync()
                     ?? (evt.AppUserId == null ? null
                         : await _ctx.AppUsers!.AsNoTracking().Where(u => u.Id == evt.AppUserId)
                             .Select(u => new { UserId = (Guid?)u.Id, ProfileId = u.AppUserProfileId, u.UserName })
                             .FirstOrDefaultAsync());

            row = new LoginDeactivation
            {
                Id = Guid.NewGuid(), SeparationId = evt.SeparationId, SeparationCode = evt.SeparationCode,
                EmployeeId = evt.EmployeeId, EmployeeCode = evt.EmployeeCode,
                AppUserId = login?.UserId, AppUserProfileId = login?.ProfileId, UserName = login?.UserName,
                LastWorkingDate = evt.LastWorkingDate, DeactivateOn = deactivateOn, CreatedDate = now
            };
            await _ctx.LoginDeactivations!.AddAsync(row);

            if (deactivateOn <= now)
            {
                await _deactivation.DisableAsync(row);
            }
            else if (login == null)
            {
                _logger.LogInformation("No login found yet for {EmployeeCode}; deactivation stays scheduled for {DeactivateOn:u}.",
                    evt.EmployeeCode, deactivateOn);
            }
            else
            {
                await _deactivation.NotifyAdminsAsync("Leaver login scheduled for deactivation",
                    $"Login '{login.UserName}' for {evt.EmployeeCode} will be disabled on " +
                    $"{ToLocal(deactivateOn):dd MMM yyyy HH:mm} ({_cfg.TimeZoneId}) — separation {evt.SeparationCode}.", now);
                if (login.ProfileId != null)
                    await _ctx.Notifications!.AddAsync(new Notification
                    {
                        Id = Guid.NewGuid(), RecipientProfileId = login.ProfileId.Value, Type = "Offboarding",
                        Title = "Your access end date",
                        Message = $"Your SBERP access ends after your last working day ({evt.LastWorkingDate:dd MMM yyyy}).",
                        CreatedDate = now
                    });
                _logger.LogInformation("Login {UserName} for {EmployeeCode} scheduled for deactivation at {DeactivateOn:u}.",
                    login.UserName, evt.EmployeeCode, deactivateOn);
            }

            await _ctx.SaveChangesAsync();
        }

        /// <summary>00:00 local on the day after the last working day, as UTC; now when no date was given.</summary>
        private DateTime DeactivationInstantUtc(DateTime? lastWorkingDate, DateTime nowUtc)
        {
            if (lastWorkingDate == null) return nowUtc;
            var localMidnight = DateTime.SpecifyKind(lastWorkingDate.Value.Date.AddDays(1), DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(localMidnight, Zone());
        }

        private DateTime ToLocal(DateTime utc) => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone());

        private TimeZoneInfo Zone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(_cfg.TimeZoneId); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; }
            catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; }
        }
    }

    /// <summary>
    /// Disables logins whose scheduled deactivation time has arrived
    /// (LoginDeactivations.ProcessedDate IS NULL AND DeactivateOn &lt;= now).
    /// Runs every Offboarding:SweepMinutes, and once at start-up so a restart
    /// over a deadline catches up immediately.
    /// </summary>
    public sealed class LoginDeactivationScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopes;
        private readonly OffboardingSettings _cfg;
        private readonly ILogger<LoginDeactivationScheduler> _logger;

        public LoginDeactivationScheduler(IServiceScopeFactory scopes, IOptions<OffboardingSettings> cfg,
            ILogger<LoginDeactivationScheduler> logger)
        {
            _scopes = scopes;
            _cfg = cfg.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delay = TimeSpan.FromMinutes(Math.Max(1, _cfg.SweepMinutes));
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await SweepAsync(stoppingToken); }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Login deactivation sweep failed.");
                }
                try { await Task.Delay(delay, stoppingToken); } catch (OperationCanceledException) { return; }
            }
        }

        private async Task SweepAsync(CancellationToken ct)
        {
            using var scope = _scopes.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<SecurityDBContext>();
            var service = scope.ServiceProvider.GetRequiredService<ILoginDeactivationService>();
            var now = DateTime.UtcNow;
            var due = await ctx.LoginDeactivations!
                .Where(x => x.ProcessedDate == null && x.DeactivateOn <= now)
                .OrderBy(x => x.DeactivateOn).Take(50).ToListAsync(ct);
            foreach (var row in due)
            {
                await service.DisableAsync(row);
                await ctx.SaveChangesAsync(ct);
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SBERP.Security.Models.Base;
using SBERP.Security.Persistence;
using System.Net;
using System.Security.Cryptography;
using BCryptNet = BCrypt.Net.BCrypt;

namespace SBERP.Security.Messaging
{
    /// <summary>
    /// Provisions the login for a newly onboarded employee:
    /// <list type="number">
    /// <item>AppUserProfile (FullName, Email, default role, EmployeeId link) + AppUser
    ///   (username = employee code, random temporary password, BCrypt-hashed) —
    ///   the same shape UserService creates, so login/menus work unchanged;</item>
    /// <item>bell notifications for the new user and for every Admin;</item>
    /// <item>a welcome e-mail queued in dbo.EmailOutbox (same transaction);</item>
    /// <item>after commit, publishes AppUserProvisionedEvent so HR sets Employees.AppUserId.</item>
    /// </list>
    /// Idempotent: keyed on AppUserProfiles.EmployeeId (unique). A redelivered
    /// event finds the existing profile and only re-publishes the write-back
    /// event, which is how a failed publish gets retried.
    /// </summary>
    public sealed class EmployeeOnboardedHandler : IIntegrationEventHandler<EmployeeOnboardedEvent>
    {
        private static readonly Guid AdminRoleId = new("1B15CE5A-56B3-4EB9-8286-6E27F770B0DA");

        private readonly SecurityDBContext _ctx;
        private readonly IIntegrationEventPublisher _publisher;
        private readonly OnboardingProvisioningSettings _cfg;
        private readonly ILogger<EmployeeOnboardedHandler> _logger;

        public EmployeeOnboardedHandler(SecurityDBContext ctx, IIntegrationEventPublisher publisher,
            IOptions<OnboardingProvisioningSettings> cfg, ILogger<EmployeeOnboardedHandler> logger)
        {
            _ctx = ctx;
            _publisher = publisher;
            _cfg = cfg.Value;
            _logger = logger;
        }

        public async Task HandleAsync(EmployeeOnboardedEvent evt)
        {
            var existing = await (from p in _ctx.AppUserProfiles!.AsNoTracking()
                                  join u in _ctx.AppUsers!.AsNoTracking() on p.Id equals u.AppUserProfileId
                                  where p.EmployeeId == evt.EmployeeId
                                  select new { ProfileId = p.Id, UserId = u.Id, u.UserName }).FirstOrDefaultAsync();
            if (existing != null)
            {
                _logger.LogInformation("Login already provisioned for employee {EmployeeId} ({UserName}); re-publishing write-back.",
                    evt.EmployeeId, existing.UserName);
                PublishProvisioned(evt.EmployeeId, existing.UserId, existing.ProfileId, existing.UserName);
                return;
            }

            // out-of-order delivery guard: never create an active login for someone who has already left
            var nowUtc = DateTime.UtcNow;
            if (await _ctx.LoginDeactivations!.AnyAsync(d => d.EmployeeId == evt.EmployeeId && d.DeactivateOn <= nowUtc))
            {
                _logger.LogWarning("Employee {EmployeeCode} is already separated; login not provisioned.", evt.EmployeeCode);
                return;
            }

            var actor = _cfg.SystemActorProfileId.ToString().ToUpperInvariant();
            var now = DateTime.UtcNow;
            var fullName = string.IsNullOrWhiteSpace(evt.FullName) ? evt.EmployeeCode : evt.FullName!.Trim();
            var userName = await UniqueUserNameAsync(evt.EmployeeCode, evt.EmployeeId);
            var tempPassword = TemporaryPassword();
            var salt = BCryptNet.GenerateSalt(13);

            var profile = new AppUserProfile
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = evt.OfficialEmail,
                AppUserRoleId = _cfg.DefaultRoleId,
                JobTitle = string.IsNullOrWhiteSpace(evt.DesignationName) ? null : evt.DesignationName.Trim(),
                EmployeeId = evt.EmployeeId,
                CreatedBy = actor, CreatedDate = now, IsActive = true
            };
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                AppUserProfileId = profile.Id,
                UserName = userName,
                Password = BCryptNet.HashPassword(tempPassword, salt),
                SaltKey = salt,
                CreatedBy = actor, CreatedDate = now, IsActive = true
            };

            await using (var tx = await _ctx.Database.BeginTransactionAsync())
            {
                await _ctx.AppUserProfiles!.AddAsync(profile);
                await _ctx.AppUsers!.AddAsync(user);

                await _ctx.Notifications!.AddAsync(new Notification
                {
                    Id = Guid.NewGuid(), RecipientProfileId = profile.Id, Type = "Onboarding",
                    Title = "Welcome to SBERP",
                    Message = $"Your account ({userName}) is ready. Your temporary password was sent to your official e-mail.",
                    CreatedDate = now
                });

                var adminIds = await _ctx.AppUserProfiles!.AsNoTracking()
                    .Where(p => p.AppUserRoleId == AdminRoleId && p.IsActive == true)
                    .Select(p => p.Id).ToListAsync();
                foreach (var adminId in adminIds)
                {
                    await _ctx.Notifications!.AddAsync(new Notification
                    {
                        Id = Guid.NewGuid(), RecipientProfileId = adminId, Type = "Onboarding",
                        Title = "New employee login created",
                        Message = $"{fullName} ({evt.EmployeeCode}) was onboarded; login '{userName}' created"
                                + (string.IsNullOrWhiteSpace(evt.OfficialEmail) ? " — no e-mail on file, share credentials manually." : "."),
                        RouteLink = "/business/security/appuserprofile",
                        CreatedDate = now
                    });
                }

                if (!string.IsNullOrWhiteSpace(evt.OfficialEmail))
                {
                    await _ctx.EmailOutbox!.AddAsync(new EmailOutbox
                    {
                        Id = Guid.NewGuid(), ToAddress = evt.OfficialEmail, ToName = fullName,
                        Subject = "Welcome to SBERP — your login details", Category = "WelcomeEmail",
                        Body = WelcomeBody(fullName, evt.EmployeeCode, userName, tempPassword, evt.DateOfJoining),
                        CreatedDate = now
                    });
                }

                await _ctx.SaveChangesAsync();
                await tx.CommitAsync();
            }

            _logger.LogInformation("Provisioned login {UserName} (AppUser {UserId}) for employee {EmployeeId}.",
                userName, user.Id, evt.EmployeeId);

            // after commit: a failure here throws → the message is re-queued → the
            // idempotent branch above re-publishes.
            PublishProvisioned(evt.EmployeeId, user.Id, profile.Id, userName);
        }

        private void PublishProvisioned(Guid employeeId, Guid userId, Guid profileId, string? userName)
            => _publisher.Publish(IntegrationRoutingKeys.AppUserProvisioned, new AppUserProvisionedEvent
            {
                EmployeeId = employeeId, AppUserId = userId, AppUserProfileId = profileId,
                UserName = userName, ProvisionedOn = DateTime.UtcNow
            });

        /// <summary>Employee code (e.g. emp-0004) as the username; suffixed if already taken.</summary>
        private async Task<string> UniqueUserNameAsync(string? employeeCode, Guid employeeId)
        {
            var baseName = string.IsNullOrWhiteSpace(employeeCode)
                ? "emp-" + employeeId.ToString("N")[..8]
                : employeeCode.Trim().ToLowerInvariant();
            var name = baseName;
            for (var i = 2; await _ctx.AppUsers!.AnyAsync(u => u.UserName == name); i++)
                name = $"{baseName}-{i}";
            return name;
        }

        /// <summary>14 chars from a CSPRNG with at least one upper, lower, digit and symbol.</summary>
        private static string TemporaryPassword()
        {
            const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ", lower = "abcdefghijkmnpqrstuvwxyz",
                         digits = "23456789", symbols = "!@#$%*?";
            const string all = upper + lower + digits + symbols;
            var chars = new List<char>
            {
                upper[RandomNumberGenerator.GetInt32(upper.Length)],
                lower[RandomNumberGenerator.GetInt32(lower.Length)],
                digits[RandomNumberGenerator.GetInt32(digits.Length)],
                symbols[RandomNumberGenerator.GetInt32(symbols.Length)],
            };
            while (chars.Count < 14) chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
            // Fisher–Yates so the guaranteed classes aren't always first
            for (var i = chars.Count - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
            return new string(chars.ToArray());
        }

        private string WelcomeBody(string? fullName, string? employeeCode, string userName, string tempPassword, DateTime? joining)
        {
            string H(string? s) => WebUtility.HtmlEncode(s ?? string.Empty);
            var joinLine = joining.HasValue ? $"<p>Your joining date is <b>{joining.Value:dd MMM yyyy}</b>.</p>" : string.Empty;
            return $@"<div style=""font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333"">
<p>Dear {H(fullName)},</p>
<p>Welcome aboard! Your employee profile <b>{H(employeeCode)}</b> has been created and your SBERP login is ready.</p>
{joinLine}
<table style=""border-collapse:collapse;margin:12px 0"">
  <tr><td style=""padding:4px 12px 4px 0"">Sign-in page</td><td><a href=""{H(_cfg.LoginUrl)}"">{H(_cfg.LoginUrl)}</a></td></tr>
  <tr><td style=""padding:4px 12px 4px 0"">Username</td><td><b>{H(userName)}</b></td></tr>
  <tr><td style=""padding:4px 12px 4px 0"">Temporary password</td><td><b>{H(tempPassword)}</b></td></tr>
</table>
<p>Keep this password private. Ask your SBERP administrator to set a new password for you (Security &rarr; User Profile) if you need one. If you did not expect this e-mail, contact HR.</p>
<p>— SBERP Human Resources</p></div>";
        }
    }
}

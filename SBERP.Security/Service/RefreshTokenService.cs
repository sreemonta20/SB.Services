using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Persistence;
using SBERP.Shared.Services;

namespace SBERP.Security.Service
{
    /// <summary>Outcome of presenting a refresh token.</summary>
    public enum RefreshTokenStatus
    {
        Ok,
        /// <summary>Unknown, signed out, or used by someone other than its owner.</summary>
        Invalid,
        /// <summary>Sliding or absolute session expiry passed.</summary>
        Expired,
        /// <summary>An already rotated token came back: it was copied. The whole session is revoked.</summary>
        Reused
    }

    public sealed record RefreshTokenRotation(RefreshTokenStatus Status, AppUser? User = null, Guid? FamilyId = null, string? NewToken = null);

    /// <summary>
    /// Stores and rotates refresh tokens in AppUserRefreshTokens. See <see cref="AppUserRefreshToken"/>.
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>Starts a new login session for the user and returns its first (raw) refresh token.</summary>
        Task<string> IssueAsync(Guid appUserId, string? ip, string? userAgent);

        /// <summary>
        /// Exchanges a refresh token for its successor. <paramref name="expectedUserId"/> is the user of
        /// the access token sent with it; a token of another user is rejected.
        /// </summary>
        Task<RefreshTokenRotation> RotateAsync(string rawToken, Guid expectedUserId, string? ip, string? userAgent);

        /// <summary>Signs out the session (family) of the token. Returns its user, or <c>null</c> when not found.</summary>
        Task<Guid?> RevokeSessionAsync(string rawToken, string? ip, string reason);

        /// <summary>Revokes every token of one session.</summary>
        Task RevokeFamilyAsync(Guid familyId, string? ip, string reason);

        /// <summary>
        /// Ends every session of the user: revokes all refresh tokens and rejects the access tokens
        /// already issued (in every microservice, through the shared blacklist).
        /// </summary>
        Task EndAllSessionsAsync(Guid appUserId, string reason, string? ip = null);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        /// <summary>
        /// A token rotated this recently is treated as a race (two requests of the same tab sent it at
        /// once), not as theft: the late request is refused, but the session is kept.
        /// </summary>
        private static readonly TimeSpan ReuseGracePeriod = TimeSpan.FromSeconds(30);
        private const int DefaultSessionLifetimeDays = 7;

        private readonly SecurityDBContext _context;
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly ISecurityLogService _securityLogService;
        private readonly AppSettings _appSettings;

        public RefreshTokenService(SecurityDBContext context, ITokenService tokenService, ITokenBlacklistService tokenBlacklistService,
            ISecurityLogService securityLogService, IOptions<AppSettings> options)
        {
            _context = context;
            _tokenService = tokenService;
            _tokenBlacklistService = tokenBlacklistService;
            _securityLogService = securityLogService;
            _appSettings = options.Value;
        }

        private TimeSpan SlidingLifetime => TimeSpan.FromDays(_appSettings.RefreshTokenExpirationDays > 0 ? _appSettings.RefreshTokenExpirationDays : 1);
        private TimeSpan SessionLifetime => TimeSpan.FromDays(_appSettings.RefreshTokenSessionLifetimeDays > 0 ? _appSettings.RefreshTokenSessionLifetimeDays : DefaultSessionLifetimeDays);

        public async Task<string> IssueAsync(Guid appUserId, string? ip, string? userAgent)
        {
            DateTime now = DateTime.UtcNow;

            // housekeeping: rows of sessions that ended more than a day ago can no longer be used or reused
            DateTime purgeBefore = now.AddDays(-1);
            await _context.AppUserRefreshTokens!
                .Where(t => t.AppUserId == appUserId && t.SessionExpiresAt < purgeBefore)
                .ExecuteDeleteAsync();

            string rawToken = _tokenService.GenerateRefreshToken();
            DateTime sessionExpiresAt = now.Add(SessionLifetime);
            await _context.AppUserRefreshTokens!.AddAsync(NewToken(appUserId, rawToken, Guid.NewGuid(), now, sessionExpiresAt, ip, userAgent));
            await _context.SaveChangesAsync();
            return rawToken;
        }

        public async Task<RefreshTokenRotation> RotateAsync(string rawToken, Guid expectedUserId, string? ip, string? userAgent)
        {
            DateTime now = DateTime.UtcNow;
            string tokenHash = _tokenService.HashToken(rawToken);

            AppUserRefreshToken? token = await _context.AppUserRefreshTokens!.AsNoTracking()
                .Include(t => t.AppUser)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

            if (token == null)
                return new RefreshTokenRotation(RefreshTokenStatus.Invalid);

            if (token.AppUserId != expectedUserId)
            {
                _securityLogService.LogWarning($"Refresh token of user {token.AppUserId} was sent with an access token of user {expectedUserId}. Refused.");
                return new RefreshTokenRotation(RefreshTokenStatus.Invalid);
            }

            if (token.RevokedDate != null)
            {
                if (token.RevokedReason == ConstantSupplier.TOKEN_REVOKE_ROTATED && token.RevokedDate < now - ReuseGracePeriod)
                {
                    _securityLogService.LogWarning($"Rotated refresh token reused for user {token.AppUserId} (session {token.FamilyId}). Session revoked.");
                    await RevokeFamilyAsync(token.FamilyId, ip, ConstantSupplier.TOKEN_REVOKE_REUSE);
                    return new RefreshTokenRotation(RefreshTokenStatus.Reused, token.AppUser, token.FamilyId);
                }
                return new RefreshTokenRotation(RefreshTokenStatus.Invalid, token.AppUser, token.FamilyId);
            }

            if (token.ExpiresAt <= now || token.SessionExpiresAt <= now)
                return new RefreshTokenRotation(RefreshTokenStatus.Expired, token.AppUser, token.FamilyId);

            if (token.AppUser?.IsActive != true)
            {
                await RevokeFamilyAsync(token.FamilyId, ip, ConstantSupplier.TOKEN_REVOKE_USER_DEACTIVATED);
                return new RefreshTokenRotation(RefreshTokenStatus.Invalid, token.AppUser, token.FamilyId);
            }

            string newRawToken = _tokenService.GenerateRefreshToken();
            AppUserRefreshToken successor = NewToken(token.AppUserId, newRawToken, token.FamilyId, now, token.SessionExpiresAt, ip, userAgent);

            using IDbContextTransaction oTransaction = await _context.Database.BeginTransactionAsync();
            // conditional update: of two requests presenting the same token at once, only one rotates it
            int rotated = await _context.AppUserRefreshTokens!
                .Where(t => t.Id == token.Id && t.RevokedDate == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.RevokedDate, now)
                    .SetProperty(t => t.RevokedByIp, ip)
                    .SetProperty(t => t.RevokedReason, ConstantSupplier.TOKEN_REVOKE_ROTATED)
                    .SetProperty(t => t.ReplacedByTokenHash, successor.TokenHash));
            if (rotated != 1)
            {
                await oTransaction.RollbackAsync();
                return new RefreshTokenRotation(RefreshTokenStatus.Invalid, token.AppUser, token.FamilyId);
            }
            await _context.AppUserRefreshTokens!.AddAsync(successor);
            await _context.SaveChangesAsync();
            await oTransaction.CommitAsync();

            return new RefreshTokenRotation(RefreshTokenStatus.Ok, token.AppUser, token.FamilyId, newRawToken);
        }

        public async Task<Guid?> RevokeSessionAsync(string rawToken, string? ip, string reason)
        {
            string tokenHash = _tokenService.HashToken(rawToken);
            var token = await _context.AppUserRefreshTokens!.AsNoTracking()
                .Where(t => t.TokenHash == tokenHash)
                .Select(t => new { t.AppUserId, t.FamilyId })
                .FirstOrDefaultAsync();
            if (token == null)
                return null;

            await RevokeFamilyAsync(token.FamilyId, ip, reason);
            return token.AppUserId;
        }

        public async Task RevokeFamilyAsync(Guid familyId, string? ip, string reason)
        {
            DateTime now = DateTime.UtcNow;
            await _context.AppUserRefreshTokens!
                .Where(t => t.FamilyId == familyId && t.RevokedDate == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.RevokedDate, now)
                    .SetProperty(t => t.RevokedByIp, ip)
                    .SetProperty(t => t.RevokedReason, reason));
        }

        public async Task EndAllSessionsAsync(Guid appUserId, string reason, string? ip = null)
        {
            DateTime now = DateTime.UtcNow;
            await _context.AppUserRefreshTokens!
                .Where(t => t.AppUserId == appUserId && t.RevokedDate == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.RevokedDate, now)
                    .SetProperty(t => t.RevokedByIp, ip)
                    .SetProperty(t => t.RevokedReason, reason));

            int accessMinutes = _appSettings.AccessTokenExpirationMinutes > 0 ? _appSettings.AccessTokenExpirationMinutes : 60;
            await _tokenBlacklistService.RevokeUserTokensAsync(appUserId.ToString(), TimeSpan.FromMinutes(accessMinutes));
            _securityLogService.LogInfo($"All sessions of user {appUserId} ended ({reason}).");
        }

        private AppUserRefreshToken NewToken(Guid appUserId, string rawToken, Guid familyId, DateTime now, DateTime sessionExpiresAt,
            string? ip, string? userAgent)
        {
            DateTime slidingExpiresAt = now.Add(SlidingLifetime);
            return new AppUserRefreshToken
            {
                Id = Guid.NewGuid(),
                AppUserId = appUserId,
                TokenHash = _tokenService.HashToken(rawToken),
                FamilyId = familyId,
                ExpiresAt = slidingExpiresAt < sessionExpiresAt ? slidingExpiresAt : sessionExpiresAt,
                SessionExpiresAt = sessionExpiresAt,
                CreatedDate = now,
                CreatedByIp = Truncate(ip, 64),
                UserAgent = Truncate(userAgent, 512)
            };
        }

        private static string? Truncate(string? value, int length) =>
            value == null || value.Length <= length ? value : value[..length];
    }
}

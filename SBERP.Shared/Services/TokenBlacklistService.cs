using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SBERP.Shared.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenBlacklistService> _logger;
        private const string Prefix = "jwt_blacklist_";
        private const string UserPrefix = "jwt_user_revoked_";

        public TokenBlacklistService(
            SharedAuthCache sharedAuthCache,
            ILogger<TokenBlacklistService> logger)
        {
            _cache = sharedAuthCache.Cache;
            _logger = logger;
        }

        public async Task<bool> IsBlacklistedAsync(string tokenJti)
        {
            if (string.IsNullOrWhiteSpace(tokenJti)) return false;

            try
            {
                var value = await _cache.GetStringAsync($"{Prefix}{tokenJti}");
                return value != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable during blacklist check for JTI: {Jti}. Failing open.",
                    tokenJti);
                return false;
            }
        }

        public async Task BlacklistAsync(string tokenJti, TimeSpan expiry)
        {
            if (string.IsNullOrWhiteSpace(tokenJti)) return;

            if (expiry < TimeSpan.FromMinutes(1))
                expiry = TimeSpan.FromMinutes(1);

            try
            {
                await _cache.SetStringAsync(
                    $"{Prefix}{tokenJti}",
                    "revoked",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiry
                    });

                _logger.LogInformation(
                    "Token JTI {Jti} blacklisted. TTL: {Expiry}",
                    tokenJti, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to blacklist JTI: {Jti}. Token may be usable until expiry.",
                    tokenJti);
            }
        }

        public async Task RevokeUserTokensAsync(string userId, TimeSpan accessTokenLifetime)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            try
            {
                await _cache.SetStringAsync(
                    $"{UserPrefix}{userId.ToUpperInvariant()}",
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = accessTokenLifetime + TimeSpan.FromMinutes(1)
                    });

                _logger.LogInformation("Access tokens of user {UserId} revoked.", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to revoke access tokens of user {UserId}. They stay usable until expiry.",
                    userId);
            }
        }

        public async Task<bool> IsRevokedForUserAsync(string userId, long issuedAtUnixSeconds)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;

            try
            {
                var value = await _cache.GetStringAsync($"{UserPrefix}{userId.ToUpperInvariant()}");
                return value != null
                    && long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long revokedAt)
                    && issuedAtUnixSeconds < revokedAt;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable during user revocation check for {UserId}. Failing open.",
                    userId);
                return false;
            }
        }
    }
}

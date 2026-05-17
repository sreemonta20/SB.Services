using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenBlacklistService> _logger;

        // Key prefix stored in Redis — visible when you run: redis-cli keys SBERP*
        private const string Prefix = "jwt_blacklist_";

        public TokenBlacklistService(
            IDistributedCache cache,
            ILogger<TokenBlacklistService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> IsBlacklistedAsync(string tokenJti)
        {
            if (string.IsNullOrWhiteSpace(tokenJti)) return false;
            try
            {
                // Microsoft IDistributedCache.GetStringAsync pattern
                // Returns null if key does not exist (token not blacklisted)
                var value = await _cache.GetStringAsync($"{Prefix}{tokenJti}");
                return value != null;
            }
            catch (Exception ex)
            {
                // Fail open — Redis down must not crash the whole application.
                // Log the warning and allow the request through.
                _logger.LogWarning(ex,
                    "Redis unavailable during blacklist check for JTI: {Jti}. Failing open.",
                    tokenJti);
                return false;
            }
        }

        public async Task BlacklistAsync(string tokenJti, TimeSpan expiry)
        {
            if (string.IsNullOrWhiteSpace(tokenJti)) return;

            // Safety net: TTL must be at least 1 minute to avoid immediate expiry
            if (expiry < TimeSpan.FromMinutes(1))
                expiry = TimeSpan.FromMinutes(1);

            try
            {
                // Microsoft IDistributedCache.SetStringAsync with DistributedCacheEntryOptions
                // AbsoluteExpirationRelativeToNow matches the token's remaining lifetime
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
                // Redis unavailable — log error but do not crash the revoke flow
                _logger.LogError(ex,
                    "Failed to blacklist JTI: {Jti}. Token may be usable until expiry.",
                    tokenJti);
            }
        }
    }
}

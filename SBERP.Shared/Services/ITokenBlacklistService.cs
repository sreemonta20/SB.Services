using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Services
{
    /// <summary>
    /// Redis-backed JWT token blacklist.
    /// On logout: SBERP.Security calls BlacklistAsync() with the token's JTI.
    /// On every request: TokenBlacklistMiddleware calls IsBlacklistedAsync().
    /// </summary>
    public interface ITokenBlacklistService
    {
        /// <summary>Returns true if the token JTI is blacklisted (user logged out).</summary>
        Task<bool> IsBlacklistedAsync(string tokenJti);

        /// <summary>Adds a JTI to Redis with TTL matching the token's remaining lifetime.</summary>
        Task BlacklistAsync(string tokenJti, TimeSpan expiry);
    }
}

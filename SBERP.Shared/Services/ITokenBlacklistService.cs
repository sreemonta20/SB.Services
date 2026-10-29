using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Services
{
    /// <summary>
    /// Redis-backed JWT token blacklist, shared by all microservices (see <see cref="SharedAuthCache"/>).
    /// On logout: SBERP.Security calls BlacklistAsync() with the token's JTI.
    /// When a user's sessions are ended (password / role change, deactivation, offboarding):
    /// SBERP.Security calls RevokeUserTokensAsync().
    /// On every request: TokenBlacklistMiddleware calls IsBlacklistedAsync() and IsRevokedForUserAsync().
    /// </summary>
    public interface ITokenBlacklistService
    {
        /// <summary>Returns true if the token JTI is blacklisted (user logged out).</summary>
        Task<bool> IsBlacklistedAsync(string tokenJti);

        /// <summary>Adds a JTI to Redis with TTL matching the token's remaining lifetime.</summary>
        Task BlacklistAsync(string tokenJti, TimeSpan expiry);

        /// <summary>
        /// Rejects every access token of the user issued before now. Keep the stamp for at least the
        /// access token lifetime: older tokens expire on their own after that.
        /// </summary>
        Task RevokeUserTokensAsync(string userId, TimeSpan accessTokenLifetime);

        /// <summary>Returns true if the user's tokens were revoked after <paramref name="issuedAtUnixSeconds"/> (the token's iat).</summary>
        Task<bool> IsRevokedForUserAsync(string userId, long issuedAtUnixSeconds);
    }
}

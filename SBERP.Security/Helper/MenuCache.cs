using Microsoft.Extensions.Caching.Distributed;

namespace SBERP.Security.Helper
{
    /// <summary>
    /// Redis keys for the per-user sidebar menu (Auth/getAppUserProfileMenu).
    /// The key carries a global version, so a change to menus or role menus is picked up by every
    /// user at once by bumping the version, instead of waiting for each entry's TTL.
    /// Both calls fail open: when Redis is down the menu simply comes from the database.
    /// </summary>
    public static class MenuCache
    {
        private const string VersionKey = "user_menu_version";

        public static async Task<string> UserMenuKeyAsync(IDistributedCache cache, string userId)
        {
            string? version = null;
            try { version = await cache.GetStringAsync(VersionKey); }
            catch { /* Redis unavailable — the caller falls back to the database */ }
            return $"user_menu_{version ?? "0"}_{userId}";
        }

        public static async Task InvalidateAllAsync(IDistributedCache cache)
        {
            try { await cache.SetStringAsync(VersionKey, Guid.NewGuid().ToString("N")); }
            catch { /* Redis unavailable — nothing is cached either */ }
        }
    }
}

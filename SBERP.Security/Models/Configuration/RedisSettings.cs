namespace SBERP.Security.Models.Configuration
{
    public class RedisSettings
    {
        public string? ConnectionString { get; set; }
        public string? InstanceName { get; set; }

        /// Menu cache absolute TTL in minutes. Default 30.
        public int MenuCacheExpiryMinutes { get; set; } = 30;

        /// Sliding expiration reset window in minutes. Default 10.
        /// The TTL resets to 30 min any time the key is accessed.
        public int MenuCacheSlidingExpiryMinutes { get; set; } = 10;
    }
}

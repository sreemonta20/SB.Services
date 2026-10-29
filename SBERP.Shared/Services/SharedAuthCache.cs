using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SBERP.Shared.Services
{
    /// <summary>
    /// Redis cache for auth state that every microservice must see: revoked JWT IDs and per-user
    /// revocation stamps.
    /// <para>
    /// Each service registers its own <see cref="IDistributedCache"/> with its own InstanceName
    /// ("SBERP_Security_", "SBERP_HR_", ...), and that prefix is added to every key. A token revoked by
    /// SBERP.Security through that cache was therefore never seen by HR. This cache uses one fixed
    /// prefix for all services, on the same Redis server (RedisSettings:ConnectionString).
    /// </para>
    /// </summary>
    public sealed class SharedAuthCache : IDisposable
    {
        public const string InstanceName = "SBERP_Auth_";

        private readonly RedisCache _cache;

        public SharedAuthCache(IConfiguration configuration)
        {
            _cache = new RedisCache(Options.Create(new RedisCacheOptions
            {
                Configuration = configuration["RedisSettings:ConnectionString"] ?? "localhost:6379",
                InstanceName = InstanceName
            }));
        }

        public IDistributedCache Cache => _cache;

        public void Dispose() => _cache.Dispose();
    }
}

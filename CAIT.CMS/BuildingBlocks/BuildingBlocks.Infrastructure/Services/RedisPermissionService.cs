using BuildingBlocks.Shared.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class RedisPermissionService : IPermissionService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpPermissionFetcher _fetcher;
        private readonly string _serviceName; // اسم الخدمة الحالية (task, committee)

        public RedisPermissionService(
            IDistributedCache cache,
            IHttpPermissionFetcher fetcher,
            string serviceName)
        {
            _cache = cache;
            _fetcher = fetcher;
            _serviceName = serviceName;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission, Guid? resourceId = null)
        {
            // بناء مفتاح خاص بالخدمة والمستخدم
            // مثال: auth:perm:task:GUID
            string cacheKey = GetCacheKey(userId);

            var cachedData = await _cache.GetStringAsync(cacheKey);
            PermissionSnapshot snapshot;

            if (string.IsNullOrEmpty(cachedData))
            {
                // Cache Miss: جلب من Identity
                snapshot = await _fetcher.FetchAsync(userId, _serviceName);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30), // صلاحية الكاش 30 دقيقة
                    SlidingExpiration = TimeSpan.FromMinutes(10) // تجديد 10 دقائق عند الاستخدام
                };

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(snapshot), options);
            }
            else
            {
                // Cache Hit
                snapshot = JsonSerializer.Deserialize<PermissionSnapshot>(cachedData)!;
            }

            return snapshot.Has(permission);
        }

        public async Task InvalidateCacheAsync(Guid userId)
        {
            string cacheKey = GetCacheKey(userId);
            await _cache.RemoveAsync(cacheKey);
        }

        private string GetCacheKey(Guid userId) => $"auth:perm:{_serviceName}:{userId}";
    }
}

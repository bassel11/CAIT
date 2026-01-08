using BuildingBlocks.Shared.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class RedisPermissionService : IPermissionService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpPermissionFetcher _fetcher;
        private readonly string _serviceName; // اسم الخدمة الحالية لتمييز الطلب

        public RedisPermissionService(
            IDistributedCache cache,
            IHttpPermissionFetcher fetcher,
            string serviceName)
        {
            _cache = cache;
            _fetcher = fetcher;
            _serviceName = serviceName;
        }

        public async Task<bool> HasPermissionAsync(
            Guid userId,
            string permissionName,
            Guid? resourceId = null,
            Guid? parentResourceId = null)
        {
            // 1. مفتاح الكاش: يعتمد فقط على المستخدم (نخزن كل صلاحياته دفعة واحدة)
            // هذا يقلل عدد المفاتيح في Redis بشكل هائل
            string cacheKey = $"auth:perm:{userId}";

            PermissionSnapshot snapshot;
            var cachedJson = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedJson))
            {
                // Cache Miss: جلب كل الصلاحيات من Identity
                snapshot = await _fetcher.FetchAsync(userId, _serviceName, resourceId, parentResourceId);

                // تخزينها في Redis لمدة طويلة (ساعة مثلاً) مع Sliding Expiration
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(snapshot), options);
            }
            else
            {
                // Cache Hit
                snapshot = JsonSerializer.Deserialize<PermissionSnapshot>(cachedJson)!;
            }

            // 2. الفحص يتم محلياً في الذاكرة (سريع جداً)
            return snapshot.Has(permissionName, resourceId, parentResourceId);
        }

        public async Task InvalidateCacheAsync(Guid userId)
        {
            string cacheKey = $"auth:perm:{userId}";
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
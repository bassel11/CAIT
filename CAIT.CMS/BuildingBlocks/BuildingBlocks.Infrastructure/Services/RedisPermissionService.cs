using BuildingBlocks.Shared.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class RedisPermissionService : IPermissionService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpPermissionFetcher _fetcher;
        private readonly string _serviceName; // اسم الخدمة الحالية (Task, Committee)

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
            // بناء مفتاح الكاش: auth:perm:userId
            // ملاحظة: سيتم إضافة بادئة الخدمة تلقائياً بفضل InstanceName
            string cacheKey = $"auth:perm:{userId}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            PermissionSnapshot snapshot;

            if (string.IsNullOrEmpty(cachedData))
            {
                // Cache Miss: جلب من Identity
                snapshot = await _fetcher.FetchAsync(userId, _serviceName);

                // تخزين الكائن كاملاً في Redis
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60), // ساعة واحدة
                    SlidingExpiration = TimeSpan.FromMinutes(30) // تمديد 20 دقيقة عند الاستخدام
                };

                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(snapshot), options);
            }
            else
            {
                // Cache Hit: تحويل النص المخزن إلى كائن
                snapshot = JsonSerializer.Deserialize<PermissionSnapshot>(cachedData)!;
            }

            // الفحص يتم في الذاكرة باستخدام الدالة الذكية التي كتبناها في Shared
            return snapshot.Has(permission, resourceId);
        }

        public async Task InvalidateCacheAsync(Guid userId)
        {
            string cacheKey = $"auth:perm:{userId}";
            await _cache.RemoveAsync(cacheKey);
        }
    }
}

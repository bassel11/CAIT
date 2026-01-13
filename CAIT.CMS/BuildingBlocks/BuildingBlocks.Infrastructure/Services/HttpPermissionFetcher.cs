using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net; // 👈 ضروري للوصول إلى HttpStatusCode
using System.Net.Http.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class HttpPermissionFetcher : IHttpPermissionFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpPermissionFetcher> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpPermissionFetcher(HttpClient httpClient,
                                    ILogger<HttpPermissionFetcher> logger,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PermissionSnapshot> FetchAsync(
            Guid userId,
            string serviceName,
            Guid? resourceId,
            Guid? parentResourceId)
        {
            var securityStamp = _httpContextAccessor.HttpContext?.User?
                .FindFirst("AspNet.Identity.SecurityStamp")?.Value;

            var url = $"api/permission/snapshot?userId={userId}&service={serviceName}";

            // 🔥 6. إضافة البصمة للرابط (فقط إذا وجدت)
            // نستخدم WebUtility.UrlEncode لضمان عدم وجود رموز تكسر الرابط
            if (!string.IsNullOrEmpty(securityStamp))
            {
                url += $"&securityStamp={WebUtility.UrlEncode(securityStamp)}";
            }

            try
            {
                var response = await _httpClient.GetAsync(url);

                // 🔥 التعديل الجوهري هنا: التحقق من 401 قبل أي شيء
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning($"⛔ Identity Service returned 401 Unauthorized for user {userId}. Session expired.");

                    // نرمي هذا الاستثناء تحديداً ليصعد للأعلى ويتحول لـ 401
                    throw new UnauthorizedAccessException("User session has expired or is invalid.");
                }

                // التحقق من باقي الأخطاء (500, 404, 400)
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Identity Service returned {response.StatusCode} for user {userId}. Details: {errorContent}");

                    throw new IdentityServiceUnavailableException($"Identity Service error. Status: {response.StatusCode}");
                }

                var snapshot = await response.Content.ReadFromJsonAsync<PermissionSnapshot>();

                if (snapshot == null)
                {
                    _logger.LogWarning($"⚠️ Identity Service returned null snapshot for user {userId}.");
                    throw new IdentityServiceUnavailableException("Identity Service returned empty data.");
                }

                return snapshot;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Network Error: Could not connect to Identity Service for user {userId}");
                throw new IdentityServiceUnavailableException("Could not reach Identity Service.", ex);
            }
            // ملاحظة: لا تقم بعمل catch لـ UnauthorizedAccessException هنا، دعها تصعد
        }
    }
}
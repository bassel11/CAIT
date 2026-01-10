using BuildingBlocks.Shared.Authorization;
using BuildingBlocks.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class HttpPermissionFetcher : IHttpPermissionFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpPermissionFetcher> _logger;

        public HttpPermissionFetcher(HttpClient httpClient, ILogger<HttpPermissionFetcher> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PermissionSnapshot> FetchAsync(
            Guid userId,
            string serviceName,
            Guid? resourceId,
            Guid? parentResourceId)

        {
            var url = $"api/permission/snapshot?userId={userId}&service={serviceName}";

            try
            {
                // 1. استخدام GetAsync بدلاً من GetFromJsonAsync للتحقق من الحالة أولاً
                var response = await _httpClient.GetAsync(url);

                // 2. التحقق الصارم من النجاح
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // تسجيل الخطأ للإدارة
                    _logger.LogError($"❌ Identity Service returned {response.StatusCode} for user {userId}. Details: {errorContent}");

                    // ⚠️ هنا التغيير الجوهري: نرمي خطأ ولا نعيد كائن فارغ
                    // هذا سيوقف الكود ولن يسمح لخدمة Redis بتخزين نتيجة خاطئة
                    throw new IdentityServiceUnavailableException($"Identity Service is unavailable or returned an error. Status: {response.StatusCode}");
                }

                // 3. قراءة البيانات
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
                // هذا الخطأ يحدث عند انقطاع الشبكة تماماً (Connection Refused)
                _logger.LogError(ex, $"Network Error: Could not connect to Identity Service for user {userId}");
                throw new IdentityServiceUnavailableException("Could not reach Identity Service.", ex);
            }
            // ملاحظة: لا تلتقط IdentityServiceUnavailableException هنا، دعها تصعد للأعلى
        }
    }
}

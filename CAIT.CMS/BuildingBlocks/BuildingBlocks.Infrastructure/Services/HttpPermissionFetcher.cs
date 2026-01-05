using BuildingBlocks.Shared.Authorization;
using System.Net.Http.Json;

namespace BuildingBlocks.Infrastructure.Services
{
    public class HttpPermissionFetcher : IHttpPermissionFetcher
    {
        private readonly HttpClient _httpClient;

        public HttpPermissionFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PermissionSnapshot> FetchAsync(Guid userId, string serviceName)
        {
            //var url = $"api/permission/fetch?userId={userId}&service={serviceName}";
            //return await _client.GetFromJsonAsync<PermissionSnapshot>(url)
            //       ?? new PermissionSnapshot();

            // نفترض أن Identity لديه Endpoint بهذا الشكل
            // GET /api/permissions/snapshot?userId=...&service=task
            var url = $"api/permissions/snapshot?userId={userId}&service={serviceName}";

            try
            {
                var snapshot = await _httpClient.GetFromJsonAsync<PermissionSnapshot>(url);
                return snapshot ?? new PermissionSnapshot { UserId = userId };
            }
            catch (Exception ex)
            {
                // في حالة الخطأ، نرجع كائن فارغ للأمان (Fail Safe) أو نرمي الخطأ حسب السياسة
                // الأفضل هنا تسجيل الخطأ وإرجاع فارغ لعدم توقف النظام
                return new PermissionSnapshot { UserId = userId };
            }
        }
    }
}

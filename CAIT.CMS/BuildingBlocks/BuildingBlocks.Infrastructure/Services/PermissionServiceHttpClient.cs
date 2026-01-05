//using BuildingBlocks.Shared.Authorization;
//using System.Net.Http.Json;

//namespace BuildingBlocks.Infrastructure.Services
//{
//    public class PermissionServiceHttpClient : IPermissionService
//    {
//        private readonly HttpClient _client;

//        public PermissionServiceHttpClient(HttpClient client)
//        {
//            _client = client;
//        }

//        public async Task<bool> HasPermissionAsync(Guid userId, string permission, Guid? resourceId = null)
//        {
//            var url = $"api/permission/check?userId={userId}&permission={permission}";
//            if (resourceId.HasValue) url += $"&resourceId={resourceId}";

//            try
//            {
//                var response = await _client.GetFromJsonAsync<PermissionResponse>(url);
//                return response?.Allowed ?? false;
//            }
//            catch
//            {
//                // في حال فشل الاتصال بخدمة الهوية، نعتبره رفضاً للأمان
//                return false;
//            }
//        }

//        private class PermissionResponse
//        {
//            public bool Allowed { get; set; }
//        }
//    }
//}

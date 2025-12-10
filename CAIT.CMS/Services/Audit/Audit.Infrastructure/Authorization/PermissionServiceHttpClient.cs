using Audit.Application.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace Audit.Infrastructure.Authorization
{
    public class PermissionServiceHttpClient : IPermissionService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        public PermissionServiceHttpClient(HttpClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission, Guid? resourceId = null)
        {
            //string cacheKey = $"perm_{userId}_{permission}_{resourceId}";
            //if (_cache.TryGetValue(cacheKey, out bool allowed))
            //    return allowed;

            var url = $"api/permission/check?userId={userId}&permission={permission}";
            if (resourceId.HasValue) url += $"&resourceId={resourceId}";

            var response = await _client.GetFromJsonAsync<PermissionResponse>(url);
            var allowed = response?.Allowed ?? false;

            //_cache.Set(cacheKey, allowed, TimeSpan.FromMinutes(1));
            return allowed;
        }

        private class PermissionResponse
        {
            public bool Allowed { get; set; }
        }
    }

}

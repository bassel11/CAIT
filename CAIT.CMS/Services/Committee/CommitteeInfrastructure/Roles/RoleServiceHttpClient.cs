using CommitteeApplication.Interfaces.Roles;
using CommitteeApplication.Responses.Roles;
using System.Net.Http.Json;

namespace CommitteeInfrastructure.Roles
{
    public class RoleServiceHttpClient : IRoleServiceHttpClient
    {
        private readonly HttpClient _http;

        public RoleServiceHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken)
        {
            var result = await _http.GetFromJsonAsync<List<RoleResponse>>(
                "/api/GetRoles",
                cancellationToken
            );

            return result ?? new List<RoleResponse>();
        }
    }
}

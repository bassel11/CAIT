using CommitteeApplication.Responses.Roles;

namespace CommitteeApplication.Interfaces.Roles
{
    public interface IRoleServiceHttpClient
    {
        Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken);
    }
}

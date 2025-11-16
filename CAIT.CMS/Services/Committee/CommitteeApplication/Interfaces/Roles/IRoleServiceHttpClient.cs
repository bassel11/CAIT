using CommitteeApplication.Features.Roles.Queries.Results;

namespace CommitteeApplication.Interfaces.Roles
{
    public interface IRoleServiceHttpClient
    {
        Task<List<RoleResponse>> GetRolesAsync(CancellationToken cancellationToken);
    }
}

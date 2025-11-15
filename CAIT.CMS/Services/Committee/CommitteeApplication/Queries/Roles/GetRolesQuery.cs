using CommitteeApplication.Responses.Roles;
using MediatR;

namespace CommitteeApplication.Queries.Roles
{
    public class GetRolesQuery : IRequest<List<RoleResponse>>
    {
    }
}

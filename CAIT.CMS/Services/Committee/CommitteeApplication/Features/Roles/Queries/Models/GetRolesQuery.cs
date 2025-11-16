using CommitteeApplication.Features.Roles.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.Roles.Queries.Models
{
    public class GetRolesQuery : IRequest<List<RoleResponse>>
    {
    }
}

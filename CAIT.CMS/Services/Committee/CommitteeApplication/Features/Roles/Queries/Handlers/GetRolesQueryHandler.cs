using CommitteeApplication.Features.Roles.Queries.Models;
using CommitteeApplication.Features.Roles.Queries.Results;
using CommitteeApplication.Interfaces.Roles;
using MediatR;

namespace CommitteeApplication.Features.Roles.Queries.Handlers
{
    public class GetRolesQueryHandler
        : IRequestHandler<GetRolesQuery, List<RoleResponse>>
    {
        private readonly IRoleServiceHttpClient _roleServiceHttpClient;

        public GetRolesQueryHandler(IRoleServiceHttpClient roleServiceHttpClient)
        {
            _roleServiceHttpClient = roleServiceHttpClient;
        }

        public async Task<List<RoleResponse>> Handle(
            GetRolesQuery request,
            CancellationToken cancellationToken)
        {
            return await _roleServiceHttpClient.GetRolesAsync(cancellationToken);
        }
    }
}

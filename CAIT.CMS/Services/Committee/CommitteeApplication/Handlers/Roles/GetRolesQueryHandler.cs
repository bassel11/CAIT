using CommitteeApplication.Interfaces.Roles;
using CommitteeApplication.Queries.Roles;
using CommitteeApplication.Responses.Roles;
using MediatR;

namespace CommitteeApplication.Handlers.Roles
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

using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Models;
using CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Queries.Handlers
{
    public class GetCommiMembRolesQueryHandler
        : IRequestHandler<GetCommiMembRolesQuery, IEnumerable<GetCommiMembRolesResponse>>
    {
        private readonly ICommitteeMemberRoleRepository _rolesRepo;

        public GetCommiMembRolesQueryHandler(ICommitteeMemberRoleRepository rolesRepo)
        {
            _rolesRepo = rolesRepo;
        }

        public async Task<IEnumerable<GetCommiMembRolesResponse>> Handle(GetCommiMembRolesQuery request, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}

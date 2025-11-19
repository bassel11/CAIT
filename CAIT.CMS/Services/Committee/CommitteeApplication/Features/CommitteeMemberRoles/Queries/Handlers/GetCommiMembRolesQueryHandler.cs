using AutoMapper;
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
        private readonly IMapper _mapper;

        public GetCommiMembRolesQueryHandler(ICommitteeMemberRoleRepository rolesRepo, IMapper mapper)
        {
            _rolesRepo = rolesRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetCommiMembRolesResponse>> Handle(GetCommiMembRolesQuery request, CancellationToken cancellationToken)
        {
            var commitMembRolesList = await _rolesRepo.GetRolesByMemberIdAsync(request.CommitteeMemberId);
            return _mapper.Map<IEnumerable<GetCommiMembRolesResponse>>(commitMembRolesList);
        }
    }
}

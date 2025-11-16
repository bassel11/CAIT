using AutoMapper;
using CommitteeApplication.Features.CommitteeMembers.Queries.Models;
using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using CommitteeCore.Repositories;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Handlers
{
    public class GetComMembersListQueryHandler : IRequestHandler<GetComMembersListQuery, List<CommitteeMemberResponse>>
    {
        #region Fields

        private readonly ICommitteeMemberRepository _committeeMemberRepository;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public GetComMembersListQueryHandler(ICommitteeMemberRepository committeeMemberRepository, IMapper mapper)
        {
            _committeeMemberRepository = committeeMemberRepository;
            _mapper = mapper;
        }

        #endregion


        #region Actions

        public async Task<List<CommitteeMemberResponse>> Handle(GetComMembersListQuery request, CancellationToken cancellationToken)
        {
            var committeeMembersList = await _committeeMemberRepository.GetCommitteesMembersByCommId(request.CommitteeId);
            return _mapper.Map<List<CommitteeMemberResponse>>(committeeMembersList);
        }

        #endregion
    }
}

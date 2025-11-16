using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Models
{
    public class GetComMembersListQuery : IRequest<List<CommitteeMemberResponse>>
    {
        public Guid CommitteeId { get; set; }
        public GetComMembersListQuery(Guid committeeId)
        {
            CommitteeId = committeeId;
        }
    }
}

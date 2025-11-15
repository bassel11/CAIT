using CommitteeApplication.Responses;
using MediatR;

namespace CommitteeApplication.Queries
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

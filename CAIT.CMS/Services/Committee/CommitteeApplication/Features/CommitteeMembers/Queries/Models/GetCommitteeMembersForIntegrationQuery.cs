using CommitteeApplication.Features.CommitteeMembers.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Models
{
    public class GetCommitteeMembersForIntegrationQuery : IRequest<List<CommitteeMemberIntegrationResponse>>
    {
        public Guid CommitteeId { get; set; }
        public GetCommitteeMembersForIntegrationQuery(Guid committeeId)
        {
            CommitteeId = committeeId;
        }
    }
}

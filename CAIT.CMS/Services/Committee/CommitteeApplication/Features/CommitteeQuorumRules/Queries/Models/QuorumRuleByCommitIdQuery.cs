using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models
{
    public class QuorumRuleByCommitIdQuery : IRequest<CommitteeQuorumRuleResponse?>
    {
        public Guid CommitteeId { get; set; }

        public QuorumRuleByCommitIdQuery(Guid committeeId)
        {
            CommitteeId = committeeId;
        }
    }
}

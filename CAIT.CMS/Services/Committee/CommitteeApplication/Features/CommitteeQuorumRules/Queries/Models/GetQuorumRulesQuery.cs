using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models
{
    public class GetQuorumRulesQuery : IRequest<IEnumerable<CommitteeQuorumRuleResponse>>
    {
    }
}

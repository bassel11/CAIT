using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Results;
using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models
{
    public class GetQuorumRuleByIdQuery : IRequest<CommitteeQuorumRuleResponse?>
    {
        public Guid Id { get; set; }

        public GetQuorumRuleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}

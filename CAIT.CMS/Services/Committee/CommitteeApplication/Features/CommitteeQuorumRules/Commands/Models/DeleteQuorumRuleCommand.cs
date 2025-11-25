using MediatR;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models
{
    public class DeleteQuorumRuleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}

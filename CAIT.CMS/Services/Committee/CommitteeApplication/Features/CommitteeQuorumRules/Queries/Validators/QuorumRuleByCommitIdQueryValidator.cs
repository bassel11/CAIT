using CommitteeApplication.Features.CommitteeQuorumRules.Queries.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Queries.Validators
{
    public class QuorumRuleByCommitIdQueryValidator : AbstractValidator<QuorumRuleByCommitIdQuery>
    {
        public QuorumRuleByCommitIdQueryValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty();
        }
    }
}

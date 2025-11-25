using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Validators
{
    public class UpdateQuorumRuleCommandValidator : AbstractValidator<UpdateQuorumRuleCommand>
    {
        public UpdateQuorumRuleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Type).IsInEnum();
        }
    }
}

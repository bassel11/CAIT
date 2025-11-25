using CommitteeApplication.Features.CommitteeQuorumRules.Commands.Models;
using CommitteeCore.Enums;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeQuorumRules.Commands.Validators
{
    public class CreateQuorumRuleCommandValidator : AbstractValidator<CreateQuorumRuleCommand>
    {
        public CreateQuorumRuleCommandValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty();

            RuleFor(x => x.Type).IsInEnum();

            When(x => x.Type == QuorumType.Percentage, () =>
            {
                RuleFor(x => x.ThresholdPercent)
                    .NotNull().InclusiveBetween(1, 100);
            });

            When(x => x.Type == QuorumType.AbsoluteNumber, () =>
            {
                RuleFor(x => x.AbsoluteCount)
                    .NotNull().GreaterThan(0);
            });
        }
    }
}

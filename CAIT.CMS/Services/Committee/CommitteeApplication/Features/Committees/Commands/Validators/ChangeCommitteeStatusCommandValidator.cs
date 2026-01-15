using CommitteeApplication.Features.Committees.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.Committees.Commands.Validators
{
    public class ChangeCommitteeStatusCommandValidator : AbstractValidator<ChangeCommitteeStatusCommand>
    {
        public ChangeCommitteeStatusCommandValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty();

            RuleFor(x => x.NewStatusId)
                .GreaterThan(0).WithMessage("Invalid Status Id.");

            // [هام] لا يمكن تغيير حالة لجنة بدون نص قرار يوضح السبب
            RuleFor(x => x.DecisionText)
                .NotEmpty().WithMessage("Decision text is required for status change audit.");
        }
    }
}

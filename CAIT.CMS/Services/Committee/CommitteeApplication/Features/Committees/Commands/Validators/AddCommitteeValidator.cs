using CommitteeApplication.Features.Committees.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.Committees.Commands.Validators
{
    public class AddCommitteeValidator : AbstractValidator<AddCommitteeCommand>
    {
        public AddCommitteeValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty()
                .WithMessage("{Name} is required.")
                .NotNull()
                .MaximumLength(70)
                .WithMessage("{Name} must not exceed 70 characters.");
            RuleFor(o => o.Budget)
                .NotEmpty()
                .WithMessage("{Budget} is required.")
                .GreaterThan(-1)
                .WithMessage("{Budget} should not be -ve");
            RuleFor(o => o.Purpose)
                .NotEmpty()
                .WithMessage("{Purpose} is required");
            RuleFor(o => o.Scope)
                .NotEmpty()
                .NotNull()
                .WithMessage("{Scope} is required");
            RuleFor(x => x.StatusId)
            .GreaterThan(0)
            .WithMessage("StatusId must be greater than 0.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .When(x => x.EndDate.HasValue)
                .WithMessage("End Date must be after Start Date.");
        }
    }
}

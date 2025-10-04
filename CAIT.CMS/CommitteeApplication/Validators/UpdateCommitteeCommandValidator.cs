using CommitteeApplication.Commands;
using FluentValidation;

namespace CommitteeApplication.Validators
{
    public class UpdateCommitteeCommandValidator : AbstractValidator<UpdateCommitteeCommand>
    {
        public UpdateCommitteeCommandValidator()
        {
            RuleFor(o => o.Id)
            .NotEmpty()
            .WithMessage("{Id} is required"); // يتأكد أنه ليس Guid.Empty
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
        }
    }
}

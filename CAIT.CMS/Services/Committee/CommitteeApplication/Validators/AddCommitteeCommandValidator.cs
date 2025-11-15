using CommitteeApplication.Commands.Committee;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeApplication.Validators
{
    public class AddCommitteeCommandValidator : AbstractValidator<AddCommitteeCommand>
    {
        public AddCommitteeCommandValidator()
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
        }
    }
}

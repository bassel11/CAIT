using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Validators
{
    public class RemoCommitMembsCommandValidator : AbstractValidator<RemoveCommitteeMembersCommand>
    {
        public RemoCommitMembsCommandValidator()
        {
            RuleFor(x => x.CommitteeId)
                .NotEmpty().WithMessage("CommitteeId is required.");

            RuleFor(x => x.MembersIds)
                .NotNull().WithMessage("MembersIds list is required.")
                .NotEmpty().WithMessage("At least one user must be provided.");

            RuleFor(x => x.MembersIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("Duplicate MembersIds are not allowed.");
        }
    }
}

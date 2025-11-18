using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Validators
{
    public class AddCommitMembRolesValidator : AbstractValidator<AddCommiMembRolesCommand>
    {
        public AddCommitMembRolesValidator()
        {
            RuleFor(x => x.CommitteeMemberId)
                .NotEmpty().WithMessage("CommitteeMemberId is required");

            RuleFor(x => x.RoleIds)
                .NotNull().NotEmpty().WithMessage("At least one role must be provided")
                .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("Duplicate roles are not allowed");
        }
    }
}

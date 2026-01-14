using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Validators
{
    public class AddCommitteeMemberValidator : AbstractValidator<AddCommitteeMemberCommand>
    {
        public AddCommitteeMemberValidator()
        {
            RuleFor(x => x.CommitteeId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Affiliation).NotEmpty();

            // التحقق من الأدوار
            RuleFor(x => x.RoleIds)
                .NotNull()
                .NotEmpty().WithMessage("At least one role must be assigned.")
                .Must(r => r.Distinct().Count() == r.Count).WithMessage("Duplicate roles are not allowed.");
        }
    }
}

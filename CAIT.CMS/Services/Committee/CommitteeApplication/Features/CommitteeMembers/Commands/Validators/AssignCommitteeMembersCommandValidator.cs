using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Validators
{
    public class AsgnCommitMembsCommandValidator : AbstractValidator<AssignCommitteeMembersCommand>
    {
        public AsgnCommitMembsCommandValidator()
        {
            // CommitteeId
            RuleFor(x => x.CommitteeId)
                .NotEmpty().WithMessage("CommitteeId is required.");

            // Members collection validation
            RuleFor(x => x.Members)
                .NotNull().WithMessage("Members list is required.")
                .NotEmpty().WithMessage("At least one member must be provided.");

            // Validate each member object
            RuleForEach(x => x.Members)
                .SetValidator(new AsgnCommitMemberDtoValidator());

            // Prevent duplicate UserId in the same request
            RuleFor(x => x.Members)
                .Must(members => members.Select(m => m.UserId).Distinct().Count() == members.Count)
                .WithMessage("Duplicate UserId values are not allowed in the Members list.");
        }
    }

    public class AsgnCommitMemberDtoValidator : AbstractValidator<AssignCommitteeMemberDto>
    {
        public AsgnCommitMemberDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Affiliation)
                .NotEmpty().WithMessage("Affiliation is required.");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("RoleIds list is required.")
                .NotEmpty().WithMessage("At least one role must be assigned to the member.");

            // ContactDetails optional → no validation
        }
    }
}

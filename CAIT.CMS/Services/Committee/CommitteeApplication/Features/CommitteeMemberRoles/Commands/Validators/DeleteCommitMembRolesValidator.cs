using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Validators
{
    public class DeleteCommitMembRolesValidator : AbstractValidator<DeleteCommiMembRolesCommand>
    {
        public DeleteCommitMembRolesValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

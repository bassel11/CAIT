using CommitteeApplication.Features.CommitteeMemberRoles.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Validators
{
    public class UpdateCommitMembRolesValidator : AbstractValidator<UpdateCommiMembRolesCommand>
    {
        public UpdateCommitMembRolesValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}

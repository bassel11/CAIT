using CommitteeApplication.Features.CommitteeMembers.Commands.Models;
using FluentValidation;

namespace CommitteeApplication.Features.CommitteeMembers.Commands.Validators
{
    public class UpdateCommitteeMemberValidator : AbstractValidator<UpdateCommitteeMemberCommand>
    {
        public UpdateCommitteeMemberValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Affiliation)
                .NotEmpty().WithMessage("Affiliation is required.")
                .MaximumLength(100).WithMessage("Affiliation must not exceed 100 characters.");

            RuleFor(x => x.ContactDetails)
                .MaximumLength(200);

            // التحقق المنطقي للتواريخ
            //RuleFor(x => x.LeaveDate)
            //    .GreaterThan(x => x.JoinDate)
            //    .When(x => x.JoinDate.HasValue && x.LeaveDate.HasValue)
            //    .WithMessage("Leave Date must be after Join Date.");
        }
    }
}

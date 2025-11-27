using FluentValidation;
using MeetingApplication.Features.MoMs.Commands.Models;

namespace MeetingApplication.Features.MoMs.Commands.Validators
{
    public class SubmitMoMForApprovalCommandValidator
        : AbstractValidator<SubmitMoMForApprovalCommand>
    {
        public SubmitMoMForApprovalCommandValidator()
        {
            RuleFor(x => x.MoMId).NotEmpty();
        }
    }
}

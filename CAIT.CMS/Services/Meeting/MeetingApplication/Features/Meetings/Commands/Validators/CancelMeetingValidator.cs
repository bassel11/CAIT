using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class CancelMeetingValidator : AbstractValidator<CancelMeetingCommand>
    {
        public CancelMeetingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Cancellation reason is required.")
                .MinimumLength(5).WithMessage("Reason must be descriptive.");
        }
    }
}

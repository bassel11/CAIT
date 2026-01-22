using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class ScheduleMeetingCommandValidator : AbstractValidator<ScheduleMeetingCommand>
    {
        public ScheduleMeetingCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Meeting Id is required.");
        }
    }
}

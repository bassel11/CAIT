using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class CreateMeetingValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingValidator()
        {
            RuleFor(x => x.Title)
                            .NotEmpty()
                            .MaximumLength(500);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("StartDate must be earlier than EndDate");

            RuleFor(x => x.RecurrenceType)
                .Must(x => Enum.TryParse(typeof(RecurrenceType), x, true, out _))
                .WithMessage("Invalid RecurrenceType");
        }
    }
}

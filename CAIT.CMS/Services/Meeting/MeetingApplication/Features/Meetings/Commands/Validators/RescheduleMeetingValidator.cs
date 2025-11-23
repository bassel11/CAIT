using FluentValidation;
using MeetingApplication.Features.Meetings.Commands.Models;

namespace MeetingApplication.Features.Meetings.Commands.Validators
{
    public class RescheduleMeetingValidator : AbstractValidator<RescheduleMeetingCommand>
    {
        public RescheduleMeetingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
        }
    }
}

using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class ConfirmAttendanceValidator : AbstractValidator<ConfirmAttendanceCommand>
    {
        public ConfirmAttendanceValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.MemberId).NotEmpty();
            RuleFor(x => x.RSVP).IsInEnum();
        }
    }
}

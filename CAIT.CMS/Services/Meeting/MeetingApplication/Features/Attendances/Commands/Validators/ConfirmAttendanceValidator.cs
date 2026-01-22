using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class ConfirmAttendanceValidator : AbstractValidator<ConfirmAttendanceCommand>
    {
        public ConfirmAttendanceValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid RSVP status.")
                .NotEqual(RSVPStatus.Pending).WithMessage("You must select a valid response (Accepted, Declined, etc.).");
        }
    }
}

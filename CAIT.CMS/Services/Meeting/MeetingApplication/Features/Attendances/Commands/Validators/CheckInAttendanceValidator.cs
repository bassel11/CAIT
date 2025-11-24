using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class CheckInAttendanceValidator : AbstractValidator<CheckInAttendanceCommand>
    {
        public CheckInAttendanceValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.MemberId).NotEmpty();
            RuleFor(x => x.AttendanceStatus).IsInEnum();
        }
    }
}

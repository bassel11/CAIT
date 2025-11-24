using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class RemoveAttendanceValidator : AbstractValidator<RemoveAttendanceCommand>
    {
        public RemoveAttendanceValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}

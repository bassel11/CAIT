using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class RemoveAttendeeValidator : AbstractValidator<RemoveAttendeeCommand>
    {
        public RemoveAttendeeValidator()
        {
            RuleFor(x => x.MeetingId)
                .NotEmpty().WithMessage("MeetingId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}

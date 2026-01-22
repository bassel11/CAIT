using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class AddAttendeeValidator : AbstractValidator<AddAttendeeCommand>
    {
        public AddAttendeeValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();

            // ✅ التحقق من أن القيمة موجودة فعلاً في الـ Enum
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid Role value.");

            RuleFor(x => x.VotingRight)
                .IsInEnum().WithMessage("Invalid VotingRight value.");
        }
    }
}

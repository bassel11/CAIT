using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class CheckInAttendeeValidator : AbstractValidator<CheckInAttendeeCommand>
    {
        public CheckInAttendeeValidator()
        {
            // التحقق من المعرفات ضروري جداً لتجنب Guid.Empty
            RuleFor(x => x.MeetingId)
                .NotEmpty().WithMessage("MeetingId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            // ملاحظة:
            // لا نحتاج للتحقق من IsRemote لأنها bool
            // فهي إما true أو false ولا يمكن أن تكون قيمة خاطئة
        }
    }
}

using FluentValidation;
using MeetingApplication.Features.Attendances.Commands.Models;
using MeetingCore.Enums.AttendanceEnums;

namespace MeetingApplication.Features.Attendances.Commands.Validators
{
    public class BulkCheckInValidator : AbstractValidator<BulkCheckInCommand>
    {
        public BulkCheckInValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("The list of attendees cannot be empty.")
                .Must(items => items.Count <= 100).WithMessage("Cannot process more than 100 items at once."); // حماية الأداء

            // التحقق من كل عنصر داخل القائمة
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.UserId).NotEmpty();

                item.RuleFor(x => x.Status)
                    .IsInEnum().WithMessage("Invalid Attendance Status.")
                    .NotEqual(AttendanceStatus.None).WithMessage("Status cannot be None.");
            });
        }
    }
}

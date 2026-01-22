using FluentValidation;
using MeetingApplication.Features.AgendaItems.Commands.Models;

namespace MeetingApplication.Features.AgendaItems.Commands.Validators
{
    public class UpdateAgendaItemValidator : AbstractValidator<UpdateAgendaItemCommand>
    {
        public UpdateAgendaItemValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty().WithMessage("MeetingId is required.");
            RuleFor(x => x.AgendaItemId).NotEmpty().WithMessage("AgendaItemId is required.");

            // التحقق من صحة البيانات الشكلية
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(1).WithMessage("SortOrder must be at least 1.");

            // ملاحظة: التحقق من أن المدة > 0 موجود بالفعل داخل Duration Value Object
            // ولكن يمكن وضعه هنا أيضاً لتحسين تجربة المستخدم (Fail Fast)
            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0).When(x => x.DurationMinutes.HasValue)
                .WithMessage("Duration must be greater than zero.");
        }
    }
}

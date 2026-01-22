using FluentValidation;
using MeetingApplication.Features.MoMActionItemDrafts.Commands.Models;

namespace MeetingApplication.Features.MoMActionItemDrafts.Commands.Validators
{
    public class AddActionItemDraftCommandValidator : AbstractValidator<AddActionItemDraftCommand>
    {
        public AddActionItemDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();

            RuleFor(x => x.TaskTitle)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(300);

            // التحقق من التاريخ (اختياري، لكن يفضل أن يكون في المستقبل إذا وجد)
            // ملاحظة: في المحاضر أحياناً نوثق مهام بدأت بالفعل، لذا سأتركها مرنة، 
            // ولكن يمكن إضافة شرط: .GreaterThan(DateTime.UtcNow.Date)
        }
    }

    public class UpdateActionItemDraftCommandValidator : AbstractValidator<UpdateActionItemDraftCommand>
    {
        public UpdateActionItemDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.ActionItemId).NotEmpty();

            RuleFor(x => x.TaskTitle)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(300);
        }
    }

    public class RemoveActionItemDraftCommandValidator : AbstractValidator<RemoveActionItemDraftCommand>
    {
        public RemoveActionItemDraftCommandValidator()
        {
            RuleFor(x => x.MeetingId).NotEmpty();
            RuleFor(x => x.ActionItemId).NotEmpty();
        }
    }
}

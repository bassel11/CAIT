using BuildingBlocks.Shared.CQRS;
using FluentValidation;
using TaskCore.Enums;

namespace TaskApplication.Features.Tasks.Commands.UpdateTask
{
    public record UpdateTaskCommand(
        Guid TaskId,
        string Title,
        string Description,
        TaskPriority Priority,
        TaskCategory Category,
        DateTime? Deadline
    ) : ICommand<UpdateTaskResult>;

    public record UpdateTaskResult(Guid TaskId);

    public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskCommandValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(2000);
            RuleFor(x => x.Priority).IsInEnum();
            RuleFor(x => x.Category).IsInEnum();

            // تحقق أن الموعد الجديد (إذا وجد) في المستقبل
            RuleFor(x => x.Deadline)
                .Must(d => d == null || d > DateTime.UtcNow)
                .WithMessage("New deadline must be in the future.");
        }
    }

    public record UpdateTaskRequest(
            string Title,
            string Description,
            TaskCore.Enums.TaskPriority Priority,
            TaskCore.Enums.TaskCategory Category,
            DateTime? Deadline
        );

}

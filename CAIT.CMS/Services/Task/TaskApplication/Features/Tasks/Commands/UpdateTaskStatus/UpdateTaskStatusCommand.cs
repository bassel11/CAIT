using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Tasks.Commands.UpdateTaskStatus
{
    public record UpdateTaskStatusCommand(
    Guid TaskId,
    TaskCore.Enums.TaskStatus NewStatus) : ICommand<UpdateTaskStatusResult>;

    public record UpdateTaskStatusResult(Guid TaskId);

    public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
    {
        public UpdateTaskStatusCommandValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid task status value.");
        }
    }

    public record UpdateTaskStatusRequest(TaskCore.Enums.TaskStatus NewStatus);
}

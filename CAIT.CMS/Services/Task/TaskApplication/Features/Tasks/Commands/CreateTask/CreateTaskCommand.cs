using BuildingBlocks.Shared.CQRS;
using FluentValidation;
using TaskCore.Enums;

namespace TaskApplication.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(
        string Title,
        string Description,
        DateTime? Deadline,
        TaskPriority Priority,
        TaskCategory Category,
        Guid CommitteeId,
        Guid? MeetingId
    ) : ICommand<CreateTaskResult>;

    public record CreateTaskResult(Guid Id);

    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {

        }
    }

}

using BuildingBlocks.Shared.CQRS;
using FluentValidation;
using TaskApplication.Dtos;
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
        Guid? MeetingId,
        Guid? DecisionId,
        Guid? MoMId,
        List<AssigneeDto> Assignees
    ) : ICommand<CreateTaskResult>;

    public record CreateTaskResult(Guid Id);

    public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskCommandValidator()
        {

        }
    }

}

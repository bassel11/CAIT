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
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.CommitteeId)
                .NotEmpty().WithMessage("Committee ID is required.");

            // مثال لقاعدة معقدة
            RuleFor(x => x.Deadline)
                .GreaterThan(DateTime.UtcNow).When(x => x.Deadline.HasValue)
                .WithMessage("Deadline must be in the future.");

            RuleFor(x => x.Priority)
             .IsInEnum()
             .WithMessage("Priority must be Low (1), Medium (2), or High (3).");

            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Category must be Strategic (1), Operational (2), or Compliance (3).");
        }
    }

}

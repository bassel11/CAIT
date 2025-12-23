using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Notes.Commands.AddTaskNote
{
    public record AddTaskNoteCommand(Guid TaskId, string Content) : ICommand<AddTaskNoteResult>;

    public record AddTaskNoteResult(Guid Id);
    // 2. Validator
    public class AddTaskNoteCommandValidator : AbstractValidator<AddTaskNoteCommand>
    {
        public AddTaskNoteCommandValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty().MaximumLength(2000).WithMessage("Note content cannot be empty or exceed 2000 chars.");
        }
    }

    public record AddNoteRequest(string Content);
}

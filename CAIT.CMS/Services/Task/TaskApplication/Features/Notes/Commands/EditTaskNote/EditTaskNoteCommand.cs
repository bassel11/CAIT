using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Notes.Commands.EditTaskNote
{
    public record EditTaskNoteCommand(Guid TaskId, Guid NoteId, string NewContent)
        : ICommand<EditTaskNoteResult>;

    public record EditTaskNoteResult(Guid Id);
    public class EditTaskNoteCommandValidator : AbstractValidator<EditTaskNoteCommand>
    {
        public EditTaskNoteCommandValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty();
            RuleFor(x => x.NoteId).NotEmpty();
            RuleFor(x => x.NewContent).NotEmpty().MaximumLength(2000);
        }
    }

    public record EditNoteRequest(string Content);
}

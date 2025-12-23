using BuildingBlocks.Shared.CQRS;
using FluentValidation;

namespace TaskApplication.Features.Notes.Commands.RemoveTaskNote
{
    public record RemoveTaskNoteCommand(Guid TaskId, Guid NoteId)
        : ICommand<RemoveTaskNoteResult>;

    public record RemoveTaskNoteResult(Guid TaskId);

    public class RemoveTaskNoteCommandValidator : AbstractValidator<RemoveTaskNoteCommand>
    {
        public RemoveTaskNoteCommandValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty();
            RuleFor(x => x.NoteId).NotEmpty();
        }
    }
}

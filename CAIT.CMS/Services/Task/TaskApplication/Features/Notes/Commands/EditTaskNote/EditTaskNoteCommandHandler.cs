using BuildingBlocks.Shared.Services;
using MediatR;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Notes.Commands.EditTaskNote
{
    public class EditTaskNoteCommandHandler
        : IRequestHandler<EditTaskNoteCommand, EditTaskNoteResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public EditTaskNoteCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<EditTaskNoteResult> Handle(EditTaskNoteCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // الدومين يتحقق: هل المستخدم هو المالك؟ هل الملاحظة محذوفة؟
            task.EditNote(
                UserId.Of(_currentUser.UserId),
                TaskNoteId.Of(request.NoteId),
                request.NewContent
            );

            await _repository.SaveChangesAsync(cancellationToken);

            return new EditTaskNoteResult(request.NoteId);
        }
    }
}

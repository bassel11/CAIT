using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Notes.Commands.RemoveTaskNote
{
    public class RemoveTaskNoteCommandHandler : ICommandHandler<RemoveTaskNoteCommand, RemoveTaskNoteResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public RemoveTaskNoteCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<RemoveTaskNoteResult> Handle(RemoveTaskNoteCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // الدومين يتحقق من الملكية ويقوم بوضع علامة الحذف
            task.RemoveNote(
                UserId.Of(_currentUser.UserId),
                TaskNoteId.Of(request.NoteId)
            );

            await _repository.SaveChangesAsync(cancellationToken);

            return new RemoveTaskNoteResult(request.NoteId);
        }
    }
}

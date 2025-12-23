using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Notes.Commands.AddTaskNote
{
    public class AddTaskNoteCommandHandler : ICommandHandler<AddTaskNoteCommand, AddTaskNoteResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public AddTaskNoteCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<AddTaskNoteResult> Handle(AddTaskNoteCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // استدعاء الدومين (سيقوم بالتحقق مما إذا كان المستخدم معيناً للمهمة)
            task.AddNote(UserId.Of(_currentUser.UserId), request.Content);

            await _repository.SaveChangesAsync(cancellationToken);

            // إرجاع معرف الملاحظة الجديدة
            var lastNote = task.TaskNotes.Last().Id.Value;
            return new AddTaskNoteResult(lastNote);
        }
    }
}

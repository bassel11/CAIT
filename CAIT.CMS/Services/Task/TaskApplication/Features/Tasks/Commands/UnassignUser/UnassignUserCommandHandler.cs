using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.UnassignUser
{
    public class UnassignUserCommandHandler : ICommandHandler<UnassignUserCommand, UnassignUserResult>
    {
        private readonly ITaskRepository _repository;

        public UnassignUserCommandHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<UnassignUserResult> Handle(UnassignUserCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // استدعاء الدومين
            task.UnassignUser(UserId.Of(request.UserId));

            await _repository.SaveChangesAsync(cancellationToken);
            return new UnassignUserResult(request.UserId);
        }
    }
}

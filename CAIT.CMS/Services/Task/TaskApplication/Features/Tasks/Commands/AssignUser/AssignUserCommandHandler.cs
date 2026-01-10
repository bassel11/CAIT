using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Services;
using TaskApplication.Common.Interfaces;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.AssignUser
{
    public class AssignUserCommandHandler
        : ICommandHandler<AssignUserCommand, AssignUserResult>
    {
        private readonly ITaskRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public AssignUserCommandHandler(ITaskRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }
        public async Task<AssignUserResult> Handle(AssignUserCommand request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null)
                throw new NotFoundException("Task", request.TaskId);

            // استدعاء الدومين
            task.AssignUser(
                UserId.Of(request.UserId),
                request.Email,
                request.Name
            );

            await _repository.SaveChangesAsync(cancellationToken);
            return new AssignUserResult(task.Id.Value);
        }
    }
}

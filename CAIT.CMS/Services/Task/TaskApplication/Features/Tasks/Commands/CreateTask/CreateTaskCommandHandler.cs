using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskCore.Entities;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Commands.CreateTask
{
    public class CreateTaskCommandHandler
        : ICommandHandler<CreateTaskCommand, CreateTaskResult>
    {
        private readonly ITaskRepository _repository;

        public CreateTaskCommandHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateTaskResult> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = TaskItem.Create(
                TaskItemId.Of(Guid.NewGuid()),
                TaskTitle.Of(request.Title),
                TaskDescription.Of(request.Description),
                request.Deadline.HasValue ? TaskDeadline.Of(request.Deadline.Value) : null,
                request.Priority,
                request.Category,
                CommitteeId.Of(request.CommitteeId),
                meetingId: request.MeetingId.HasValue ? MeetingId.Of(request.MeetingId.Value) : null,
                request.DecisionId is null ? null : DecisionId.Of(request.DecisionId.Value),
                request.MoMId is null ? null : MoMId.Of(request.MoMId.Value)
            );

            foreach (var a in request.Assignees)
                task.AssignUser(UserId.Of(a.UserId), a.Email, a.Name);

            await _repository.AddAsync(task, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return new CreateTaskResult(task.Id.Value);
        }
    }
}

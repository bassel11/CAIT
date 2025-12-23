using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskApplication.Dtos;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Tasks.Queries.GetTaskDetails
{
    public class GetTaskDetailsQueryHandler : IQueryHandler<GetTaskDetailsQuery, TaskDetailsDto>
    {
        private readonly ITaskRepository _repository;

        public GetTaskDetailsQueryHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<TaskDetailsDto> Handle(GetTaskDetailsQuery request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found");

            // Mapping (Manual for explicit control, or use Mapster)
            return new TaskDetailsDto(
                task.Id.Value,
                task.Title.Value,
                task.Description.Value,
                task.Status.ToString(),
                task.Priority.ToString(),
                task.Category.ToString(),
                task.Deadline?.Value,
                task.TaskAssignees.Select(a => new TaskAssigneeDto(a.UserId.Value, a.Name, a.Email)).ToList()
            );
        }
    }
}

using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskApplication.Dtos;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Histories.Queries.GetHistory
{
    public class GetTaskHistoryQueryHandler : IQueryHandler<GetTaskHistoryQuery, List<TaskHistoryDto>>
    {
        private readonly ITaskRepository _repository;

        public GetTaskHistoryQueryHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskHistoryDto>> Handle(GetTaskHistoryQuery request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // تحويل إلى DTO
            return task.TaskHistories
                .OrderByDescending(h => h.Timestamp)
                .Select(h => new TaskHistoryDto(
                    h.Id.Value,
                    h.Action.ToString(),
                    h.Details,
                    h.UserId.Value,
                    "Unknown User", // في بيئة حقيقية نقوم بجلب الاسم من خدمة الهوية
                    h.Timestamp
                )).ToList();
        }
    }
}

using BuildingBlocks.Shared.CQRS;
using TaskApplication.Common.Interfaces;
using TaskApplication.Dtos;
using TaskCore.ValueObjects;

namespace TaskApplication.Features.Notes.Queries.GetNotes
{
    public class GetTaskNotesQueryHandler : IQueryHandler<GetTaskNotesQuery, List<TaskNoteDto>>
    {
        private readonly ITaskRepository _repository;

        public GetTaskNotesQueryHandler(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskNoteDto>> Handle(GetTaskNotesQuery request, CancellationToken cancellationToken)
        {
            var task = await _repository.GetByIdAsync(TaskItemId.Of(request.TaskId), cancellationToken);
            if (task == null) throw new KeyNotFoundException("Task not found.");

            // التحويل إلى DTO
            // ملاحظة: الفلترة للمحذوفات تتم تلقائياً بفضل Global Query Filter في EF Core Configuration
            // ولكن إذا لم يكن مفعلاً، يجب إضافة .Where(n => !n.IsDeleted)
            return task.TaskNotes
                .OrderByDescending(n => n.CreatedAt) // الأحدث أولاً
                .Select(n => new TaskNoteDto(
                    n.Id.Value,
                    n.Content,
                    n.UserId.Value,
                    n.CreatedAt
                )).ToList();
        }
    }
}

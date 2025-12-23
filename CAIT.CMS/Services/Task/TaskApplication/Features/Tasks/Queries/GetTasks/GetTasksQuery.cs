using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Pagination;
using TaskApplication.Dtos;
using TaskCore.Enums;

namespace TaskApplication.Features.Tasks.Queries.GetTasks
{
    public record GetTasksQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        TaskCore.Enums.TaskStatus? Status = null,
        TaskPriority? Priority = null,
        Guid? AssignedToUserId = null,
        Guid? CommitteeId = null,
        Guid? MeetingId = null,
        Guid? DecisionId = null,
        Guid? MoMId = null
    ) : IQuery<PaginatedResult<TaskListItemDto>>;
}

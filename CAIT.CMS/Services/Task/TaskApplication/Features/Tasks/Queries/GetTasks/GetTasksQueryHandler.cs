using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using TaskApplication.Data;
using TaskApplication.Dtos;

namespace TaskApplication.Features.Tasks.Queries.GetTasks
{
    public class GetTasksQueryHandler
        : IQueryHandler<GetTasksQuery, PaginatedResult<TaskListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTasksQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<TaskListItemDto>> Handle(
            GetTasksQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.TaskItems.AsNoTracking();

            // Filtering
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(t =>
                    t.Title.Value.Contains(request.SearchTerm) ||
                    t.Description.Value.Contains(request.SearchTerm));
            }

            if (request.Status.HasValue)
                query = query.Where(t => t.Status == request.Status);

            if (request.Priority.HasValue)
                query = query.Where(t => t.Priority == request.Priority);

            if (request.CommitteeId.HasValue)
            {
                var committeeId = TaskCore.ValueObjects.CommitteeId.Of(request.CommitteeId.Value);
                query = query.Where(t => t.CommitteeId == committeeId);
            }

            if (request.AssignedToUserId.HasValue)
            {
                var userId = TaskCore.ValueObjects.UserId.Of(request.AssignedToUserId.Value);
                query = query.Where(t => t.TaskAssignees.Any(a => a.UserId == userId));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TaskListItemDto(
                    t.Id.Value,
                    t.Title.Value,
                    t.Status.ToString(),
                    t.Priority.ToString(),
                    t.Category.ToString(),
                    t.Deadline != null ? t.Deadline.Value : null,
                    t.CommitteeId.Value,
                    t.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TaskListItemDto>(
                request.PageNumber,
                request.PageSize,
                totalCount,
                items
            );
        }
    }
}

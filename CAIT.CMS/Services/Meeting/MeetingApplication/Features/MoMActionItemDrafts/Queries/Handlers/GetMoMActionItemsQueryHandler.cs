using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Models;
using MeetingApplication.Features.MoMActionItemDrafts.Queries.Results;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMActionItemDrafts.Queries.Handlers
{
    public class GetMoMActionItemsQueryHandler
        : IQueryHandler<GetMoMActionItemsQuery, PaginatedResult<MoMActionItemResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMoMActionItemsQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<MoMActionItemResponse>> Handle(GetMoMActionItemsQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            // 1. Drill-Down
            var query = _dbContext.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .SelectMany(m => m.ActionItems)
                .AsQueryable();

            // 2. Searching (البحث في العنوان)
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var search = req.Search.Trim().ToLower();
                query = query.Where(a => a.TaskTitle.Contains(search));
            }

            // 3. Dynamic Filtering (فلترة حسب الموظف)
            if (req.Filters != null && req.Filters.ContainsKey("assigneeId"))
            {
                if (Guid.TryParse(req.Filters["assigneeId"], out var assigneeId))
                {
                    query = query.Where(a => a.AssigneeId == assigneeId);
                }
            }

            // 4. Sorting
            if (!string.IsNullOrWhiteSpace(req.SortBy))
            {
                switch (req.SortBy.ToLower())
                {
                    case "title": query = query.OrderBy(a => a.TaskTitle); break;
                    case "duedate": query = query.OrderBy(a => a.DueDate); break;
                    default: query = query.OrderBy(a => a.SortOrder); break;
                }
            }
            else
            {
                query = query.OrderBy(a => a.SortOrder);
            }

            // 5. Pagination & Projection
            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(a => new MoMActionItemResponse
                {
                    TaskTitle = a.TaskTitle,
                    AssigneeId = a.AssigneeId,
                    DueDate = a.DueDate,
                    SortOrder = a.SortOrder,
                    Status = a.Status.ToString()
                })
                .ToListAsync(ct);

            return PaginatedResult<MoMActionItemResponse>.Success(items, totalCount, req.PageNumber, req.PageSize);
        }
    }
}

using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.AgendaItems.Queries.Models;
using MeetingApplication.Features.AgendaItems.Queries.Results;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.AgendaItems.Queries.Handlers
{
    public class GetAgendaItemsQueryHandler
        : IQueryHandler<GetAgendaItemsQuery, PaginatedResult<AgendaItemResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetAgendaItemsQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<AgendaItemResponse>> Handle(GetAgendaItemsQuery request, CancellationToken cancellationToken)
        {
            var meetingId = MeetingId.Of(request.MeetingId);

            // 1. Base Query
            var query = _dbContext.AgendaItems
                .AsNoTracking()
                .Where(x => x.MeetingId == meetingId)
                .AsQueryable();

            // 2. Search (Optional)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(x => ((string)(object)x.Title).Contains(search) ||
                                 (x.Description != null && x.Description.Contains(search)));
                //query = query.Where(x => x.Title.Value.Contains(search) || (x.Description != null && x.Description.Contains(search)));
            }

            // 3. Sorting (Default by SortOrder)
            query = query.OrderBy(x => x.SortOrder);

            // 4. Count
            var totalCount = await query.CountAsync(cancellationToken);

            // 5. Pagination & Projection
            var items = await query
                .Skip(((request.PageNumber < 1 ? 1 : request.PageNumber) - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AgendaItemResponse
                {
                    Id = x.Id.Value,
                    MeetingId = x.MeetingId.Value,
                    Title = x.Title.Value,
                    Description = x.Description,
                    SortOrder = x.SortOrder.Value,
                    DurationMinutes = x.AllocatedTime != null
                        ? (int)x.AllocatedTime.Value.TotalMinutes
                        : null,

                    PresenterId = x.PresenterId != null
                        ? x.PresenterId.Value
                        : null
                })
                .ToListAsync(cancellationToken);

            // 6. Return
            return PaginatedResult<AgendaItemResponse>.Success(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

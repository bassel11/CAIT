using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Models;
using MeetingApplication.Features.MoMDecisionDrafts.Queries.Results;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.MoMDecisionDrafts.Queries.Handlers
{
    public class GetMoMDecisionsQueryHandler
        : IQueryHandler<GetMoMDecisionsQuery, PaginatedResult<MoMDecisionResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMoMDecisionsQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<MoMDecisionResponse>> Handle(GetMoMDecisionsQuery req, CancellationToken ct)
        {
            var meetingId = MeetingId.Of(req.MeetingId);

            // 1. Drill-Down: الدخول المباشر لجدول القرارات
            var query = _dbContext.Minutes
                .AsNoTracking()
                .Where(m => m.MeetingId == meetingId)
                .SelectMany(m => m.Decisions)
                .AsQueryable();

            // 2. Searching (البحث في العنوان أو النص)
            if (!string.IsNullOrWhiteSpace(req.Search))
            {
                var search = req.Search.Trim().ToLower();
                query = query.Where(d => d.Title.Contains(search) || d.Text.Contains(search));
            }

            // 3. Sorting (الترتيب)
            if (!string.IsNullOrWhiteSpace(req.SortBy))
            {
                switch (req.SortBy.ToLower())
                {
                    case "title": query = query.OrderBy(d => d.Title); break;
                    case "status": query = query.OrderBy(d => d.Status); break;
                    default: query = query.OrderBy(d => d.SortOrder); break; // الافتراضي
                }
            }
            else
            {
                query = query.OrderBy(d => d.SortOrder);
            }

            // 4. Pagination & Projection
            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((req.PageNumber - 1) * req.PageSize)
                .Take(req.PageSize)
                .Select(d => new MoMDecisionResponse
                {
                    Title = d.Title,
                    Text = d.Text,
                    SortOrder = d.SortOrder,
                    Status = d.Status.ToString()
                })
                .ToListAsync(ct);

            return PaginatedResult<MoMDecisionResponse>.Success(items, totalCount, req.PageNumber, req.PageSize);
        }
    }
}

using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.ValueObjects;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class GetMeetingsQueryHandler
        : IQueryHandler<GetMeetingsQuery, PaginatedResult<GetMeetingResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMeetingsQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<GetMeetingResponse>> Handle(GetMeetingsQuery request, CancellationToken cancellationToken)
        {
            // 1. بناء الـ Queryable (لم يتم التنفيذ في DB بعد)
            var query = _dbContext.Meetings.AsNoTracking().AsQueryable();

            // 2. الفلترة (Filtering)
            if (request.CommitteeId.HasValue)
            {
                var commId = CommitteeId.Of(request.CommitteeId.Value);
                query = query.Where(m => m.CommitteeId == commId);
            }

            // 3. البحث (Search)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                // EF Core يترجم الوصول للـ Value Object Property بشكل صحيح
                query = query.Where(m =>
                    m.Title.Value.Contains(search) ||
                    (m.Description != null && m.Description.Contains(search))
                );
            }

            // 4. الفرز (Sorting) - افتراضياً الأحدث
            query = query.OrderByDescending(x => x.CreatedAt);

            // 5. العد (Count) - ينفذ جملة SELECT COUNT(*) في الداتابيز
            var totalCount = await query.CountAsync(cancellationToken);

            // 6. جلب الصفحة (Pagination & Projection)
            // استخدام Select هنا يضمن أننا نجلب فقط الحقول المطلوبة للصفحة الحالية
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new GetMeetingResponse
                {
                    Id = m.Id.Value,
                    Title = m.Title.Value,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Status = m.Status.ToString(),
                    FormattedLocation = m.Location.Type == LocationType.Online
                        ? "Online"
                        : (m.Location.RoomName ?? m.Location.Address ?? "Physical Location"),
                    AttendeesCount = m.Attendances.Count
                })
                .ToListAsync(cancellationToken);

            // 7. إرجاع النتيجة
            return PaginatedResult<GetMeetingResponse>.Success(
                    items,
                    totalCount,
                    request.PageNumber,
                    request.PageSize
                );
        }
    }
}

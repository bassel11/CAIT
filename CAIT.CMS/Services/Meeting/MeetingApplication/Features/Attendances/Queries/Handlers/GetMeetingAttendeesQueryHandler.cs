using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class GetMeetingAttendeesQueryHandler : IQueryHandler<GetMeetingAttendeesQuery, PaginatedResult<AttendanceResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMeetingAttendeesQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<AttendanceResponse>> Handle(GetMeetingAttendeesQuery request, CancellationToken cancellationToken)
        {
            var meetingId = MeetingId.Of(request.MeetingId);

            // 1. بناء الاستعلام الأساسي (Drill-down from Meeting -> Attendances)
            var query = _dbContext.Meetings
                .AsNoTracking()
                .Where(m => m.Id == meetingId)
                .SelectMany(m => m.Attendances) // الانتقال للجدول الفرعي
                .AsQueryable();

            // 2. البحث (Search)
            // ملاحظة: الحضور يحتوي IDs فقط، البحث في الأسماء يتطلب Join مع جدول المستخدمين
            // هنا سنبحث في حالة الحضور أو الدور كنص
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                // هذا مجرد مثال، قد لا يكون البحث هنا فعالاً بدون أسماء
                // query = query.Where(a => a.AttendanceStatus.ToString().ToLower().Contains(search)); 
            }

            // 3. الفلترة (Filters)
            if (request.Filters != null && request.Filters.Any())
            {
                // مثال: تصفية حسب الحالة (Present, Absent...)
                if (request.Filters.ContainsKey("status") && Enum.TryParse<AttendanceStatus>(request.Filters["status"], true, out var status))
                {
                    query = query.Where(a => a.AttendanceStatus == status);
                }
            }

            // 4. الترتيب (SortBy)
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "status": query = query.OrderBy(a => a.AttendanceStatus); break;
                    case "checkintime": query = query.OrderByDescending(a => a.CheckInTime); break;
                    default: query = query.OrderBy(a => a.UserId); break;
                }
            }
            else
            {
                query = query.OrderBy(a => a.UserId); // ترتيب افتراضي
            }

            // 5. العد والصفحات (Pagination)
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new AttendanceResponse
                {
                    UserId = a.UserId.Value,
                    MeetingId = a.MeetingId.Value,
                    Role = a.Role.ToString(),
                    VotingRight = a.VotingRight.ToString(),
                    RSVP = a.RSVP.ToString(),
                    Status = a.AttendanceStatus.ToString(),
                    CheckInTime = a.CheckInTime
                })
                .ToListAsync(cancellationToken);

            // 6. الإرجاع باستخدام دالة Success المعدلة
            return PaginatedResult<AttendanceResponse>.Success(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

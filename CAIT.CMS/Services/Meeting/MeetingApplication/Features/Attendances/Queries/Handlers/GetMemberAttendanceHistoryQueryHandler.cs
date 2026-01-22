using BuildingBlocks.Shared.CQRS;
using MeetingApplication.Data;
using MeetingApplication.Features.Attendances.Queries.Models;
using MeetingApplication.Features.Attendances.Queries.Results;
using MeetingCore.ValueObjects.AttendanceVO;

namespace MeetingApplication.Features.Attendances.Queries.Handlers
{
    public class GetMemberAttendanceHistoryQueryHandler : IQueryHandler<GetMemberAttendanceHistoryQuery, PaginatedResult<AttendanceResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMemberAttendanceHistoryQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<AttendanceResponse>> Handle(GetMemberAttendanceHistoryQuery request, CancellationToken cancellationToken)
        {
            var userId = UserId.Of(request.MemberId);

            // استعلام عكسي: من الاجتماعات -> الحضور -> الفلترة بالعضو
            var query = _dbContext.Meetings
                .AsNoTracking()
                .SelectMany(m => m.Attendances)
                .Where(a => a.UserId == userId);

            // الترتيب حسب الأحدث افتراضياً
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderByDescending(a => a.CheckInTime);
            }

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

            return PaginatedResult<AttendanceResponse>.Success(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

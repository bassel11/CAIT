using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Data;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.ValueObjects;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class GetMeetingsByCommitteeIdQueryHandler : IQueryHandler<GetMeetingsByCommitteeIdQuery, Result<List<GetMeetingResponse>>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMeetingsByCommitteeIdQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<GetMeetingResponse>>> Handle(GetMeetingsByCommitteeIdQuery request, CancellationToken cancellationToken)
        {
            var committeeId = CommitteeId.Of(request.CommitteeId);

            // استعلام مباشر وسريع جداً يعيد قائمة خفيفة
            var list = await _dbContext.Meetings
                .AsNoTracking()
                .Where(m => m.CommitteeId == committeeId)
                .OrderByDescending(m => m.StartDate)
                .Select(m => new GetMeetingResponse
                {
                    Id = m.Id.Value,
                    Title = m.Title.Value,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Status = m.Status.ToString(),
                    LocationType = m.Location.Type.ToString(),
                    AttendeesCount = m.Attendances.Count
                    // لا نجلب الوصف الطويل أو الروابط هنا لتخفيف حجم البيانات
                })
                .ToListAsync(cancellationToken);

            return Result<List<GetMeetingResponse>>.Success(list);
        }
    }
}

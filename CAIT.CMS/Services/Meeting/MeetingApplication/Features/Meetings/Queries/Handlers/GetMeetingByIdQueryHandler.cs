using BuildingBlocks.Shared.CQRS;
using BuildingBlocks.Shared.Wrappers;
using MeetingApplication.Data;
using MeetingApplication.Features.Meetings.Queries.Models;
using MeetingApplication.Features.Meetings.Queries.Results;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.ValueObjects.MeetingVO;

namespace MeetingApplication.Features.Meetings.Queries.Handlers
{
    public class GetMeetingByIdQueryHandler : IQueryHandler<GetMeetingByIdQuery, Result<GetMeetingResponse>>
    {
        private readonly IMeetingDbContext _dbContext;

        public GetMeetingByIdQueryHandler(IMeetingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<GetMeetingResponse>> Handle(GetMeetingByIdQuery request, CancellationToken cancellationToken)
        {
            var meetingId = MeetingId.Of(request.Id);

            // ⚡ Performance Optimization:
            // 1. AsNoTracking: لا تقم بتتبع الكائنات (قراءة فقط).
            // 2. Select: احلب فقط الأعمدة التي نحتاجها (SQL Projection).
            // 3. Flattening: تحويل ValueObjects إلى قيم بسيطة مباشرة.

            var dto = await _dbContext.Meetings
                .AsNoTracking()
                .Where(m => m.Id == meetingId)
                .Select(m => new GetMeetingResponse
                {
                    Id = m.Id.Value,
                    CommitteeId = m.CommitteeId.Value,
                    Title = m.Title.Value,
                    Description = m.Description,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Status = m.Status.ToString(),

                    // تحويل الـ Location ValueObject يدوياً للأداء
                    LocationType = m.Location.Type.ToString(),
                    LocationRoom = m.Location.RoomName,
                    LocationAddress = m.Location.Address,
                    LocationOnlineUrl = m.Location.OnlineUrl,
                    FormattedLocation = m.Location.Type == LocationType.Physical
                        ? $"{m.Location.RoomName} - {m.Location.Address}"
                        : (m.Location.Type == LocationType.Online ? "Online" : "Hybrid"),

                    // تحويل الـ Recurrence
                    IsRecurring = m.Recurrence.Type != RecurrenceType.None,
                    RecurrenceType = m.Recurrence.Type.ToString(),

                    // Audit
                    CreatedBy = m.CreatedBy == null ? Guid.Empty : Guid.Parse(m.CreatedBy),
                    CreatedAt = m.CreatedAt ?? DateTime.MinValue,

                    // Links
                    TeamsLink = m.TeamsLink,
                    OutlookEventId = m.OutlookEventId,

                    // Sub-Queries (تتحول لـ COUNT في SQL)
                    AttendeesCount = m.Attendances.Count,
                    AgendaItemsCount = m.AgendaItems.Count
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return Result<GetMeetingResponse>.Failure("Meeting not found.");

            return Result<GetMeetingResponse>.Success(dto);
        }
    }
}

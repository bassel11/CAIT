using MediatR;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public record GetAttendanceForMeetingQuery(Guid MeetingId) : IRequest<List<GetAttendanceResponse>>;
}

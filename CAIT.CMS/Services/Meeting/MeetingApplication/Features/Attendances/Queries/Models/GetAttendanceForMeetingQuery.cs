using MediatR;
using MeetingApplication.Features.Attendances.Queries.Results;

namespace MeetingApplication.Features.Attendances.Queries.Models
{
    public class GetAttendanceForMeetingQuery : IRequest<List<GetAttendanceResponse>>
    {
        public Guid MeetingId { get; set; }
    }
}

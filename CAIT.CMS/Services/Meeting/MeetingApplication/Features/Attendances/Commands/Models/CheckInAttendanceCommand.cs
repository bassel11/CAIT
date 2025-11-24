using MediatR;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public class CheckInAttendanceCommand : IRequest<Guid>
    {
        public Guid MeetingId { get; set; }
        public Guid MemberId { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
    }
}

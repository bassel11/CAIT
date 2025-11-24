using MediatR;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public class ConfirmAttendanceCommand : IRequest<Guid>
    {
        public Guid MeetingId { get; set; }
        public Guid MemberId { get; set; } // from Committee service
        public RSVPStatus RSVP { get; set; } // Yes / No / Maybe
    }
}

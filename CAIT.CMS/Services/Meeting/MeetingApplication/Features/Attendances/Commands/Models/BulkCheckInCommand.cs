using MediatR;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public class BulkCheckInCommand : IRequest<Unit>
    {
        public Guid MeetingId { get; }
        public List<(Guid MemberId, AttendanceStatus Status)> Entries { get; }

        public BulkCheckInCommand(Guid meetingId, List<(Guid MemberId, AttendanceStatus Status)> entries)
        {
            MeetingId = meetingId;
            Entries = entries;
        }
    }

}

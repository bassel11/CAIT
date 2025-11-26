using MediatR;
using MeetingCore.Enums;

namespace MeetingApplication.Features.Attendances.Commands.Models
{
    public class BulkCheckInCommand : IRequest<Unit>
    {
        public Guid MeetingId { get; }
        public List<BulkCheckInEntry> Entries { get; set; }

        public class BulkCheckInEntry
        {
            public Guid MemberId { get; set; }
            public AttendanceStatus Status { get; set; }
        }
    }

}

using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = default!;

        public Guid MemberId { get; set; } // from Committee service

        public RSVPStatus RSVP { get; set; } // Yes / No / Maybe
        public AttendanceStatus AttendanceStatus { get; set; } // None, Present, Remote, Absent

        public DateTime? Timestamp { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

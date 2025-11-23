using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class MeetingNotification
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = null!;

        public NotificationType NotificationType { get; set; } // MeetingScheduled, Rescheduled, Cancelled, Reminder

        public string PayloadJson { get; set; } = default!;

        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}

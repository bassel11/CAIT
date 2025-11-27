using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class MinutesOfMeeting
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = default!;

        public MoMStatus Status { get; set; } // Draft, PendingApproval, Approved

        public string AttendanceSummary { get; set; } = default!;
        public string AgendaSummary { get; set; } = default!;
        public string DecisionsSummary { get; set; } = default!;
        public string ActionItemsJson { get; set; } = default!; // Linked to Task service

        public int VersionNumber { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public ICollection<MinutesVersion> Versions { get; set; } = new List<MinutesVersion>();
        public ICollection<MoMAttachment> MoMAttachmentS { get; set; } = new List<MoMAttachment>();


    }
}

using MeetingCore.Enums;
using MeetingCore.Events;

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

        public List<MinutesVersion> Versions { get; private set; } = new();
        public List<MoMAttachment> MoMAttachments { get; private set; } = new();

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //public ICollection<MinutesVersion> Versions { get; set; } = new List<MinutesVersion>();
        //public ICollection<MoMAttachment> MoMAttachmentS { get; set; } = new List<MoMAttachment>();

        // Domain events collector (simple)
        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public MinutesOfMeeting()
        {

        }
        public MinutesOfMeeting(Guid id, Guid meetingId)
        {
            Id = id;
            MeetingId = meetingId;
            Status = MoMStatus.PendingApproval;
            VersionNumber = 1;
        }


        public void Approve(DateTime utcNow, Guid userId)
        {
            if (Status != MoMStatus.PendingApproval)
                throw new InvalidOperationException("Only pending MoMs can be approved.");


            Status = MoMStatus.Approved;
            ApprovedAt = utcNow;
            ApprovedBy = userId;
            UpdatedAt = utcNow;


            // Add domain event
            _events.Add(new MoMApprovedEvent(Id, MeetingId, userId, utcNow, utcNow));
        }


        public void AddAttachment(MoMAttachment attachment)
        {
            MoMAttachments.Add(attachment);
            //UpdatedAt = DateTime.UtcNow;
        }


        public void ClearEvents() => _events.Clear();

    }
}

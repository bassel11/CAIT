using BuildingBlocks.Contracts.Common;

namespace MeetingCore.Events
{
    public class MoMApprovedEvent : IDomainEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid ApprovedBy { get; }
        public DateTime ApprovedAt { get; }

        // من IDomainEvent
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public MoMApprovedEvent(Guid momId, Guid meetingId, Guid approvedBy, DateTime approvedAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
        }
    }

}

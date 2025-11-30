namespace MeetingCore.Events
{
    public class MoMApprovedEvent : IDomainEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid ApprovedBy { get; }
        public DateTime ApprovedAt { get; }
        public DateTime OccurredOn { get; }

        public MoMApprovedEvent(Guid momId, Guid meetingId, Guid approvedBy, DateTime approvedAt, DateTime occurredOn)
        {
            MoMId = momId;
            MeetingId = meetingId;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
            OccurredOn = occurredOn;
        }
    }

}

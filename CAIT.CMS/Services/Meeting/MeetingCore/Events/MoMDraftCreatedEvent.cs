namespace MeetingCore.Events
{
    public class MoMDraftCreatedEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid CreatedBy { get; }
        public DateTime CreatedAt { get; }

        public MoMDraftCreatedEvent(Guid momId, Guid meetingId, Guid createdBy, DateTime createdAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }
    }

}

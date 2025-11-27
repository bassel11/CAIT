namespace MeetingCore.Events
{
    public class MoMDraftUpdatedEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid UpdatedBy { get; }
        public DateTime UpdatedAt { get; }

        public MoMDraftUpdatedEvent(Guid momId, Guid meetingId, Guid updatedBy, DateTime updatedAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }

}

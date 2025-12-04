namespace MeetingCore.Events
{
    public class MoMApprovedIntegrationEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public MoMApprovedIntegrationEvent(Guid momId, Guid meetingId)
        {
            MoMId = momId;
            MeetingId = meetingId;
        }
    }
}

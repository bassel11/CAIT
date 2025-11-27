namespace MeetingCore.Events
{
    public class MoMApprovedEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid ApprovedBy { get; }
        public DateTime ApprovedAt { get; }

        public MoMApprovedEvent(Guid momId, Guid meetingId, Guid approvedBy, DateTime approvedAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
        }
    }

}

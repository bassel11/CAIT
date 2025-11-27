namespace MeetingCore.Events
{
    public class MoMSubmittedForApprovalEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid SubmittedBy { get; }
        public DateTime SubmittedAt { get; }

        public MoMSubmittedForApprovalEvent(Guid momId, Guid meetingId, Guid submittedBy, DateTime submittedAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            SubmittedBy = submittedBy;
            SubmittedAt = submittedAt;
        }
    }

}

using BuildingBlocks.Contracts.Audit;

namespace MeetingCore.Events
{
    public class MoMApprovedEvent : IAuditableEvent
    {
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid ApprovedBy { get; }

        // IAuditableEvent Implementation
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public string UserId => ApprovedBy.ToString();
        public string EntityName => "MinutesOfMeeting";
        public string ActionType => "Approve";
        public string PrimaryKey => MoMId.ToString();

        public MoMApprovedEvent(Guid momId, Guid meetingId, Guid approvedBy, DateTime utcNow)
        {
            MoMId = momId;
            MeetingId = meetingId;
            ApprovedBy = approvedBy;
        }
    }
}

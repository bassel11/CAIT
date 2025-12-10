using BuildingBlocks.Contracts.Audit;
using BuildingBlocks.Contracts.Common;

namespace MeetingCore.Events
{
    public class MoMApprovedEvent : IDomainEvent, IAuditLogCreated
    {
        // Domain fields
        public Guid MoMId { get; }
        public Guid MeetingId { get; }
        public Guid ApprovedBy { get; }
        public DateTime ApprovedAt { get; }

        // IDomainEvent
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        // IAuditLogCreated
        public string UserId => ApprovedBy.ToString();
        public string ServiceName => "MeetingService";
        public string EntityName => "MinutesOfMeeting";
        public string ActionType => "Approve";
        public string PrimaryKey => MoMId.ToString();
        public string? OldValues => null;
        public string? NewValues => null;
        public DateTime Timestamp => ApprovedAt;

        public MoMApprovedEvent(Guid momId, Guid meetingId, Guid approvedBy, DateTime approvedAt)
        {
            MoMId = momId;
            MeetingId = meetingId;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
        }
    }
}

namespace CommitteeCore.Entities
{
    public class CommitteeAuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }

        public Guid? CommitteeMemberId { get; set; }

        public string Action { get; set; }             // "AddMember", "UpdateStatus", etc.
        public string PerformedBy { get; set; }        // UserId

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string Details { get; set; }
        public string DecisionText { get; set; }
    }
}

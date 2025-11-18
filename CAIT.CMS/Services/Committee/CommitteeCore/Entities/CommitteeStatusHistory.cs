namespace CommitteeCore.Entities
{
    public class CommitteeStatusHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }

        // Old/New Status
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        // Linked decision
        public string DecisionText { get; set; } = string.Empty!;
        public string DecisionDocumentUrl { get; set; }
    }
}

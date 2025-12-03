namespace MeetingCore.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public bool Processed { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int Attempts { get; set; }
        public string? LastError { get; set; }

        // حقول القفل
        public string? LockedBy { get; set; }
        public DateTime? LockedAt { get; set; }

        // Concurrency Token
        public byte[] RowVersion { get; set; } = default!;

    }
}

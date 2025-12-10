using System.ComponentModel.DataAnnotations;

namespace Audit.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EventId { get; set; }           // EventId from producer
        public string EventType { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public string PrimaryKey { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? OldValues { get; set; }      // JSON
        public string? NewValues { get; set; }      // JSON
        public DateTime Timestamp { get; set; }     // when the event occurred (UTC)

        // meta
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public string RawPayload { get; set; } = string.Empty;

        // chain / tamper evidence
        public string? PreviousHash { get; set; }
        [Required]
        public string Hash { get; set; } = string.Empty;
    }
}

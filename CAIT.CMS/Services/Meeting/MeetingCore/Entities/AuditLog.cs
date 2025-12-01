namespace MeetingCore.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = default!;
        public string EntityType { get; set; } = default!;
        public Guid? EntityId { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
    }

}

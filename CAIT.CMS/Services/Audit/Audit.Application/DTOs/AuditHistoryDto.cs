namespace Audit.Application.DTOs
{
    public class AuditHistoryDto
    {
        public Guid AuditId { get; set; }
        public string Action { get; set; } // Created, Modified, Deleted
        public string User { get; set; }
        public string Email { get; set; }
        public string TimeAgo { get; set; } // "2 hours ago"
        public DateTime Timestamp { get; set; }
        public List<AuditChangeDto> Changes { get; set; } = new();
    }

    // التفاصيل الدقيقة (الفرق بين القيم)
    public class AuditChangeDto
    {
        public string FieldName { get; set; } // اسم العمود
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}

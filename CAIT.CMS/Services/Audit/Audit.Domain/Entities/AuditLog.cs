using System.ComponentModel.DataAnnotations;

namespace Audit.Domain.Entities
{
    public class AuditLog
    {
        // فهارس لضمان سرعة التقارير والفلترة [المصدر: 29]
        //[Index(nameof(CommitteeId))]
        //[Index(nameof(UserId))]
        //[Index(nameof(ActionType))]
        //[Index(nameof(Timestamp), nameof(ReceivedAt))]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // --- البيانات الأساسية ---
        public Guid EventId { get; set; }
        public string EventType { get; set; } = string.Empty;
        [MaxLength(100)] public string ServiceName { get; set; } = string.Empty;
        [MaxLength(100)] public string EntityName { get; set; } = string.Empty;
        [MaxLength(50)] public string ActionType { get; set; } = string.Empty;
        [MaxLength(100)] public string PrimaryKey { get; set; } = string.Empty;

        // [المصدر: 29] الفلترة حسب اللجنة
        [MaxLength(100)] public string? CommitteeId { get; set; }

        // [المصدر: 5] تتبع المستخدم
        [MaxLength(100)] public string UserId { get; set; } = string.Empty;
        [MaxLength(200)] public string UserName { get; set; } = string.Empty;
        [MaxLength(500)] public string? Email { get; set; }

        // [المصدر: 3] التبرير والسبب
        public string? Justification { get; set; }

        // مستوى الخطورة (لدعم التنبيهات [المصدر: 36])
        [MaxLength(20)] public string Severity { get; set; } = "Info";

        // --- تفاصيل البيانات (JSON) ---
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? ChangedColumns { get; set; }

        // --- التوقيتات ---
        public DateTime Timestamp { get; set; } // وقت حدوث الفعل
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        // --- الأمان وعدم التلاعب (Tamper-Proof) [المصدر: 20-23] ---
        public string RawPayload { get; set; } = string.Empty; // تخزين الرسالة الأصلية

        [MaxLength(64)]
        public string? PreviousHash { get; set; } // السجل السابق

        [Required]
        [MaxLength(64)]
        public string Hash { get; set; } = string.Empty; // Checksum للحماية
    }
}


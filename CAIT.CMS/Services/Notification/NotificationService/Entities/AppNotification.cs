namespace NotificationService.Entities
{
    public class AppNotification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty; // ✅ قيمة افتراضية
        public string Message { get; set; } = string.Empty; // ✅ قيمة افتراضية
        public string Link { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;

        // الحل للمشكلة التي ظهرت لك:
        public required string Type { get; set; } // ✅ استخدام required (C# 11+)
                                                  // أو
                                                  // public string Type { get; set; } = "Info"; // إعطاء قيمة افتراضية

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
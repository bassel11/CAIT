namespace BuildingBlocks.Contracts.Notifications
{
    // حدث عام لإرسال الإشعارات من أي خدمة
    public record SendNotificationIntegrationEvent
    {
        // نوع الإشعار (لأغراض التخصيص في خدمة الإشعارات)
        public string Type { get; init; } = "General";

        // المعرف المرجعي (مثل MeetingId) للربط
        public Guid ReferenceId { get; init; }

        // نص الرسالة
        public string Message { get; init; } = string.Empty;

        // من يجب أن يستلم الإشعار؟ (يمكن تطويره ليكون قائمة UserIds)
        // هنا نستخدم الدور للتبسيط، ولكن الأفضل إرسال قائمة IDs
        public string RecipientRole { get; init; } = "All";

        // توقيت الإرسال
        public DateTime SentAt { get; init; } = DateTime.UtcNow;
    }
}

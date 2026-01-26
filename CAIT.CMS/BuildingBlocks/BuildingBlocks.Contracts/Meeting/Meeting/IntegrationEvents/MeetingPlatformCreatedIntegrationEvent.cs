namespace BuildingBlocks.Contracts.Meeting.Meeting.IntegrationEvents
{
    // هذا الحدث يمثل نجاح عملية التكامل
    public record MeetingPlatformCreatedIntegrationEvent
    {
        public Guid MeetingId { get; init; }

        // معرف الحدث في تقويم Outlook (مهم جداً لعمليات التعديل والإلغاء مستقبلاً)
        public string OutlookEventId { get; init; } = default!;

        // رابط الانضمام للاجتماع (Teams Join URL)
        public string TeamsLink { get; init; } = default!;

        // حالة العملية (اختياري، للتأكيد)
        public string IntegrationStatus { get; init; } = "Success";

        // توقيت إتمام العملية
        public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;
    }
}

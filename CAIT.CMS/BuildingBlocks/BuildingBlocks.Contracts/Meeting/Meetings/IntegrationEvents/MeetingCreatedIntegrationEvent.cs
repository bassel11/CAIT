namespace BuildingBlocks.Contracts.Meeting.Meetings.IntegrationEvents
{
    public record MeetingCreatedIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid CommitteeId { get; init; }
        public string Title { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public string TimeZone { get; init; } = default!;
        public string? Location { get; init; } // يمكن إضافته
        public Guid CreatedBy { get; init; } // هنا يمكننا تحويل النص لـ Guid إذا كنا متأكدين
        public DateTime CreatedAt { get; init; }
    }
}

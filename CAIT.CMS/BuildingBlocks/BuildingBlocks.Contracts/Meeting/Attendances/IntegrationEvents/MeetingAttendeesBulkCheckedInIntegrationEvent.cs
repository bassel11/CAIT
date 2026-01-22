namespace BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents
{
    public record MeetingAttendeesBulkCheckedInIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public List<BulkCheckInItemDto> Items { get; init; } = new();
        public DateTime Timestamp { get; init; }
    }

    public record BulkCheckInItemDto(Guid MemberId, string Status);
}

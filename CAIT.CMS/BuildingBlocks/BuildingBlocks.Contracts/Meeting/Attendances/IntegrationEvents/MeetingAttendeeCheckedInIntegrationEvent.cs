namespace BuildingBlocks.Contracts.Meeting.Attendances.IntegrationEvents
{
    public record MeetingAttendeeCheckedInIntegrationEvent
    {
        public Guid MeetingId { get; init; }
        public Guid MemberId { get; init; }
        public string AttendanceStatus { get; init; } = default!;
        public bool IsRemote { get; init; }
        public DateTime Timestamp { get; init; }
    }
}

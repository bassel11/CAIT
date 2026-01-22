namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingCreatedEvent(
        Guid MeetingId,
        Guid CommitteeId,
        string Title,
        DateTime StartDate,
        DateTime EndDate,
        string TimeZone,
        string? CreatedBy,
        DateTime? CreatedAt
    ) : IDomainEvent;
}

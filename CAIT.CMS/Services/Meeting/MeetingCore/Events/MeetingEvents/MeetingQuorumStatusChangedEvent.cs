namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingQuorumStatusChangedEvent(
        Guid MeetingId,
        bool IsQuorumMet,
        DateTime Timestamp
    ) : IDomainEvent;
}

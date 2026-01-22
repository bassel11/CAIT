namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingCompletedEvent(
         Guid MeetingId,
         DateTime CompletedAt
     ) : IDomainEvent;
}

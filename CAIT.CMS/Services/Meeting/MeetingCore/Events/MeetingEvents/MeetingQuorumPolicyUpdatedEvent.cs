namespace MeetingCore.Events.MeetingEvents
{
    public record MeetingQuorumPolicyUpdatedEvent(
        Guid MeetingId,
        string OldPolicyDescription,
        string NewPolicyDescription,
        string UpdatedBy,
        DateTime Timestamp
    ) : IDomainEvent;
}

namespace MeetingCore.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }

    }
}

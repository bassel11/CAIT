namespace DecisionCore.Events
{
    public record DecisionDeletedEvent(
        DecisionId DecisionId,
        DecisionTitle Title
    ) : IDomainEvent;
}

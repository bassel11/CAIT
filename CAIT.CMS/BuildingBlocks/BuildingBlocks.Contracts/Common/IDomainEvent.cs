namespace BuildingBlocks.Contracts.Common
{
    public interface IDomainEvent
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
}

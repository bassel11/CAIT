namespace BuildingBlocks.Shared.Abstractions
{
    public interface IHasDomainEvents
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        IDomainEvent[] ClearDomainEvents();
    }
}

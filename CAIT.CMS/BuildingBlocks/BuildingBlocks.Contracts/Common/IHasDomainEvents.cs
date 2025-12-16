namespace BuildingBlocks.Contracts.Common
{
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }
        void ClearEvents();
    }
}

using MediatR;

namespace BuildingBlocks.Contracts.Common
{
    // الوراثة من INotification ضرورية لكي يراها MediatR
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        DateTime OccurredAt { get; }
    }
}

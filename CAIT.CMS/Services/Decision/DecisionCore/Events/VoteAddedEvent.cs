using DecisionCore.Abstractions;
using DecisionCore.Enums;
using DecisionCore.ValueObjects;

namespace DecisionCore.Events
{
    public record VoteAddedEvent(
    DecisionId DecisionId,
    Guid MemberId,
    VoteType VoteType
) : IDomainEvent;
}

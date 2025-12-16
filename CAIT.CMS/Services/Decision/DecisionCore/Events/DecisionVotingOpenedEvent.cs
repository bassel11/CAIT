using DecisionCore.Abstractions;
using DecisionCore.ValueObjects;

namespace DecisionCore.Events
{
    public record DecisionVotingOpenedEvent(
        DecisionId DecisionId,
        VotingDeadline VotingDeadline
    ) : IDomainEvent;
}

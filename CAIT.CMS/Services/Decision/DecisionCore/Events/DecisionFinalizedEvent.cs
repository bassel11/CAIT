using DecisionCore.Abstractions;
using DecisionCore.Enums;
using DecisionCore.ValueObjects;

namespace DecisionCore.Events
{
    public record DecisionFinalizedEvent(
        DecisionId DecisionId,
        MeetingId MeetingId,
        DecisionStatus FinalStatus,
        Guid FinalizedBy
    ) : IDomainEvent;
}

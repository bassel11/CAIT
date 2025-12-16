using DecisionCore.Abstractions;
using DecisionCore.Enums;
using DecisionCore.ValueObjects;

namespace DecisionCore.Events
{
    public record DecisionUpdatedEvent(
        DecisionId DecisionId,
        MeetingId MeetingId,
        DecisionTitle Title,
        string TextArabic,
        string TextEnglish,
        DecisionType Type,
        AgendaItemId? AgendaItemId
    ) : IDomainEvent;
}

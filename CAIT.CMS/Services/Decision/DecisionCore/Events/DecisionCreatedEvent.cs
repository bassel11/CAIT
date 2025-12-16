using DecisionCore.Abstractions;
using DecisionCore.ValueObjects;

namespace DecisionCore.Events
{
    public record DecisionCreatedEvent(
    DecisionId DecisionId,
    MeetingId MeetingId,
    DecisionTitle Title,
    string TextArabic,
    string TextEnglish) : IDomainEvent;
}

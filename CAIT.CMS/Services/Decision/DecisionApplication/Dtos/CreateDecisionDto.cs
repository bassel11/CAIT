using DecisionCore.Enums;

namespace DecisionApplication.Dtos
{
    public sealed record CreateDecisionDto(
    string Title,
    string TextArabic,
    string TextEnglish,
    Guid MeetingId,
    Guid? AgendaItemId,
    DecisionType Type
);
}

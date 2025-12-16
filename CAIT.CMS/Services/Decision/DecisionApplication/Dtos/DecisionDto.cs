using DecisionCore.Enums;

namespace DecisionApplication.Dtos
{
    public record DecisionDto(
    Guid Id,
    string Title,
    string TextArabic,
    string TextEnglish,
    Guid MeetingId,
    Guid? AgendaItemId,
    DateTime? VotingDeadline,
    DecisionType Type,
    DecisionStatus Status,
    IReadOnlyList<VoteDto> Votes
);
}

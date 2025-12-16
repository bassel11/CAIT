using DecisionCore.Enums;

namespace DecisionApplication.Dtos
{
    public record VoteDto(
    Guid Id,
    Guid MemberId,
    VoteType Type,
    DateTime VotedAt
);
}

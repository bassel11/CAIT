namespace DecisionApplication.Dtos
{
    public sealed record AddVoteDto(
    Guid MemberId,
    string VoteType
);
}

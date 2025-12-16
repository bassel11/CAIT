using DecisionApplication.Dtos;
using DecisionCore.Entities;

namespace DecisionApplication.Extensions
{
    public static class DecisionMappingExtensions
    {
        public static DecisionDto ToDecisionDto(this Decision decision)
        {
            return new DecisionDto(
                Id: decision.Id.Value,
                Title: decision.Title.Value,
                TextArabic: decision.Text.Arabic,
                TextEnglish: decision.Text.English,
                MeetingId: decision.MeetingId.Value,
                AgendaItemId: decision.AgendaItemId?.Value,
                Type: decision.Type,
                Status: decision.Status,
                VotingDeadline: decision.VotingDeadline?.Value,
                Votes: decision.Votes
                    .Select(v => new VoteDto(
                        v.Id.Value,
                        v.MemberId,
                        v.Type,
                        v.VotedAt))
                    .ToList()
            );
        }

        // هذه الإضافة تحل مشكلتك
        public static IEnumerable<DecisionDto> ToDecisionDtoList(this IEnumerable<Decision> decisions)
        {
            return decisions.Select(d => d.ToDecisionDto());
        }
    }
}

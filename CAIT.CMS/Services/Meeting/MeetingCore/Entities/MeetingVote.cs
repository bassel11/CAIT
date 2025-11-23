using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class MeetingVote
    {
        public Guid Id { get; set; }

        public Guid DecisionId { get; set; }
        public MeetingDecision Decision { get; set; } = null!;

        public Guid MemberId { get; set; } // From Committee service
        public VoteChoice Choice { get; set; } // Yes, No, Abstain

        public DateTime Timestamp { get; set; }
    }
}

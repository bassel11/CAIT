using MeetingCore.Enums;

namespace MeetingCore.Entities
{
    public class MeetingDecision
    {
        public Guid Id { get; set; }

        public Guid MeetingId { get; set; }
        public Meeting Meeting { get; set; } = default!;

        public string Text { get; set; } = default!;

        public DecisionOutcome Outcome { get; set; } // Approved, Rejected, Deferred

        public GovernanceModel GovernanceModel { get; set; } // ChairmanAuthority, Consensus, Voting

        public Guid? RelatedAgendaItemId { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<MeetingVote> Votes { get; set; } = new List<MeetingVote>();
    }
}

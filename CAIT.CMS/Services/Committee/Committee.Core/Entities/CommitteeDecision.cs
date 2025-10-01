using Committee.Core.Common;

namespace Committee.Core.Entities
{
    public class CommitteeDecision : EntityBase
    {
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public string DecisionTextArabic { get; set; }
        public string DecisionTextEnglish { get; set; }
        public Guid? MeetingId { get; set; }          // Optional link to Meeting
        public string GovernanceModel { get; set; }   // ChairmanAuthority, Consensus, Voting
        public DecisionOutcome Outcome { get; set; }  // Approved, Rejected, Deferred
    }

    public enum DecisionOutcome
    {
        Approved,
        Rejected,
        Deferred
    }
}

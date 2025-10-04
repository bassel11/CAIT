using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Entities
{
    public class CommitteeDecision
    {
        public Guid Id { get; set; } = Guid.NewGuid();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Entities
{
    public class Committee
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Scope { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CommitteeType Type { get; set; }
        public CommitteeStatus Status { get; set; } = CommitteeStatus.Draft;
        public decimal? Budget { get; set; }
        public string CreationDecisionText { get; set; } // Official decision text
        public string UpdatedDecisionText { get; set; }
        public ICollection<CommitteeMember> CommitteeMembers { get; set; }
        public ICollection<CommitteeDocument> CommitteeDocuments { get; set; }
        public ICollection<CommitteeDecision> CommitteeDecisions { get; set; }
    }

    public enum CommitteeType
    {
        Permanent = 0,
        Temporary = 1
    }

    public enum CommitteeStatus
    {
        Draft = 0,
        Active = 1,
        Suspended = 2,
        Completed = 3,
        Dissolved = 4,
        Archived = 5
    }
}

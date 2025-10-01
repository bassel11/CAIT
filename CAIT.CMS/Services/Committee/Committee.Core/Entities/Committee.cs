using Committee.Core.Common;

namespace Committee.Core.Entities
{
    public class Committee : EntityBase
    {
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
        public ICollection<CommitteeMember> Members { get; set; }
        public ICollection<CommitteeDocument> Documents { get; set; }
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

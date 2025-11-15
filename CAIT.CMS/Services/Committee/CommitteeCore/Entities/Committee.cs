using System.ComponentModel.DataAnnotations;

namespace CommitteeCore.Entities
{
    public class Committee
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        public string Purpose { get; set; }

        [Required]
        public string Scope { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CommitteeType Type { get; set; }
        public CommitteeStatus Status { get; set; } = CommitteeStatus.Draft;
        public decimal? Budget { get; set; }

        [Required]
        public string CreationDecisionText { get; set; } // Official decision text
        public string UpdatedDecisionText { get; set; }
        public ICollection<CommitteeMember> CommitteeMembers { get; set; } = new HashSet<CommitteeMember>();
        public ICollection<CommitteeDocument> CommitteeDocuments { get; set; } = new HashSet<CommitteeDocument>();
        public ICollection<CommitteeDecision> CommitteeDecisions { get; set; } = new HashSet<CommitteeDecision>();
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

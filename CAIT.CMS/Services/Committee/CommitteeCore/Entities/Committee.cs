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

        public DateTime? EndDate { get; set; }

        [Required]
        public CommitteeType Type { get; set; }
        public int StatusId { get; set; }          // FK to CommitteeStatus
        public CommitteeStatus Status { get; set; }
        public decimal? Budget { get; set; }

        [Required]
        public string CreationDecisionText { get; set; } // Official decision text
        public string? CreationDecisionDocumentUrl { get; set; }
        public string UpdatedDecisionText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CommitteeMember> CommitteeMembers { get; set; } = new HashSet<CommitteeMember>();
        public ICollection<CommitteeDocument> CommitteeDocuments { get; set; } = new HashSet<CommitteeDocument>();
        public ICollection<CommitteeDecision> CommitteeDecisions { get; set; } = new HashSet<CommitteeDecision>();
        public ICollection<CommitteeStatusHistory> CommitteeStatusHistories { get; set; } = new HashSet<CommitteeStatusHistory>();
        public ICollection<CommitteeAuditLog> CommitteeAuditLogs { get; set; } = new HashSet<CommitteeAuditLog>();
        public CommitteeQuorumRule CommitteeQuorumRule { get; set; }


    }

    public enum CommitteeType
    {
        Permanent = 0,
        Temporary = 1
    }

    //public enum CommitteeStatus
    //{
    //    Draft = 0,
    //    Active = 1,
    //    Suspended = 2,
    //    Completed = 3,
    //    Dissolved = 4,
    //    Archived = 5
    //}
}

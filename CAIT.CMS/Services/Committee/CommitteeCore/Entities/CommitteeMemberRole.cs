namespace CommitteeCore.Entities
{
    public class CommitteeMemberRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeMemberId { get; set; }
        public CommitteeMember CommitteeMember { get; set; }

        //public string Role { get; set; } // e.g., Chairman + Member
        public Guid RoleId { get; set; }      // Identity Role ID
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

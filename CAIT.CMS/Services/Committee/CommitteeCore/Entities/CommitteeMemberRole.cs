namespace CommitteeCore.Entities
{
    public class CommitteeMemberRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeMemberId { get; set; }
        public CommitteeMember CommitteeMember { get; set; }

        //public string Role { get; set; } // e.g., Chairman + Member
        public Guid RoleId { get; set; }      // Identity Role ID
    }
}

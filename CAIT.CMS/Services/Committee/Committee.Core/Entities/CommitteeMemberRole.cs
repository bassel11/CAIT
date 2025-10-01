using Committee.Core.Common;

namespace Committee.Core.Entities
{
    public class CommitteeMemberRole : EntityBase
    {
        public Guid CommitteeMemberId { get; set; }
        public CommitteeMember CommitteeMember { get; set; }
        public string Role { get; set; } // e.g., Chairman + Member
    }
}

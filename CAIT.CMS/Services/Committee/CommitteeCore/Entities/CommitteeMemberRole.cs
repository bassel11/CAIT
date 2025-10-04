using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Entities
{
    public class CommitteeMemberRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeMemberId { get; set; }
        public CommitteeMember CommitteeMember { get; set; }
        public string Role { get; set; } // e.g., Chairman + Member
    }
}

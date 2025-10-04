using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitteeCore.Entities
{
    public class CommitteeMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public Guid UserId { get; set; }                 // Reference to User Service
        public string Role { get; set; }                 // Chairman, Vice Chairman, Rapporteur, Member, Observer
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }       // Optional
        public ICollection<CommitteeMemberRole> CommitteeMemberRoles { get; set; }
    }
}

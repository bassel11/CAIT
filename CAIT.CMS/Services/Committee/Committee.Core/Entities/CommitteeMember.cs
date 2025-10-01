﻿using Committee.Core.Common;

namespace Committee.Core.Entities
{
    public class CommitteeMember : EntityBase
    {
        public Guid CommitteeId { get; set; }
        public Committee Committee { get; set; }
        public Guid UserId { get; set; }                 // Reference to User Service
        public string Role { get; set; }                 // Chairman, Vice Chairman, Rapporteur, Member, Observer
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }       // Optional
        public ICollection<CommitteeMemberRole> Roles { get; set; }
    }
}

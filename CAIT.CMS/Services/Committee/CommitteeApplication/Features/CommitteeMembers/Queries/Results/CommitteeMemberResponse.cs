namespace CommitteeApplication.Features.CommitteeMembers.Queries.Results
{
    public class CommitteeMemberResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public Guid UserId { get; set; }                 // External Reference to Identity Service on Tbale Users
        public string Role { get; set; }                 // Chairman, Vice Chairman, Rapporteur, Member, Observer
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }
    }
}

namespace CommitteeApplication.Features.CommitteeMembers.Queries.Results
{
    public class CommitteeMemberResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeId { get; set; }
        public Guid UserId { get; set; }                 // External Reference to Identity Service on Tbale Users
        public string Affiliation { get; set; }          // CAIT department or external entity
        public string ContactDetails { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }

        public string? FullNAME { get; set; }
        public string? UserEmail { get; set; }
    }
}

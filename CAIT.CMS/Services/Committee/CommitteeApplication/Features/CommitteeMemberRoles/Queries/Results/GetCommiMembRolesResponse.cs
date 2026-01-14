namespace CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results
{
    public class GetCommiMembRolesResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeMemberId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }

    }
}

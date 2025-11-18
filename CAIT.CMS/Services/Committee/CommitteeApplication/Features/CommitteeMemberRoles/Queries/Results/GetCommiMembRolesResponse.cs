namespace CommitteeApplication.Features.CommitteeMemberRoles.Queries.Results
{
    public class GetCommiMembRolesResponse
    {
        public Guid Id { get; set; }
        public Guid CommitteeMemberId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

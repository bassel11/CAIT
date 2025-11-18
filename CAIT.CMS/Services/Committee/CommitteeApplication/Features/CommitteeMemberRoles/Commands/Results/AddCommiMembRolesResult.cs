namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results
{
    public class AddCommiMembRolesResult
    {
        public Guid CommitteeMemberId { get; set; }
        // public Guid CommitteeId { get; set; }
        public List<Guid> AddedRoleIds { get; set; } = new();
    }
}

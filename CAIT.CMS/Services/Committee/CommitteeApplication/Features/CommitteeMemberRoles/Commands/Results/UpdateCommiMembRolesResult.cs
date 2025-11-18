namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results
{
    public class UpdateCommiMembRolesResult
    {
        public Guid Id { get; set; }
        public Guid CommitteeMemberId { get; set; }
        public Guid UpdatedRoleId { get; set; }
    }
}

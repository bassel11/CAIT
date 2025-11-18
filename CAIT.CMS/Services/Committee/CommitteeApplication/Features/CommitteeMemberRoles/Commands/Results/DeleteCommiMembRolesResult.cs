namespace CommitteeApplication.Features.CommitteeMemberRoles.Commands.Results
{
    public class DeleteCommiMembRolesResult
    {
        public Guid Id { get; set; }
        public Guid CommitteeMemberId { get; set; }
        public Guid DeletedRoleId { get; set; }
    }
}

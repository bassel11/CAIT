namespace CommitteeApplication.Features.CommitteeMembers.Commands.Results
{
    public class AssignCommitteeMembersResult
    {
        public List<Guid> AddedMemberIds { get; set; } = new();
    }
}

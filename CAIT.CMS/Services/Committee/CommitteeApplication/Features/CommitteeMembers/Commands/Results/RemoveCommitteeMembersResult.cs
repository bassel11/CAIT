namespace CommitteeApplication.Features.CommitteeMembers.Commands.Results
{
    public class RemoveCommitteeMembersResult
    {
        public List<Guid> RemovedMemberIds { get; set; } = new();
        public List<Guid> NotFoundMemberIds { get; set; } = new(); // optional: للتنبيه إذا كان العضو غير موجود
    }
}

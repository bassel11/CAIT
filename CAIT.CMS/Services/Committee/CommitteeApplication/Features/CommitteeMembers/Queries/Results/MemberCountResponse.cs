namespace CommitteeApplication.Features.CommitteeMembers.Queries.Results
{
    public class MemberCountResponse
    {
        public Guid CommitteeId { get; set; }
        public int ActiveMemberCount { get; set; }
    }
}

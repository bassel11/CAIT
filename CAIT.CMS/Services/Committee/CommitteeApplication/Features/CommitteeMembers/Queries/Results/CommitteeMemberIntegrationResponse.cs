namespace CommitteeApplication.Features.CommitteeMembers.Queries.Results
{
    public class CommitteeMemberIntegrationResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }

        // هذا الرقم يمثل الدور (1: رئيس، 2: نائب، إلخ) ليتم تحويله في Meeting Service
        public int CommitteeRoleId { get; set; }

        public bool HasVotingRight { get; set; }
    }
}

namespace MeetingApplication.Interfaces.Committee
{
    public interface ICommitteeService
    {
        Task<int> GetMemberCountAsync(Guid committeeId, CancellationToken ct = default);
        Task<QuorumRule?> GetQuorumRuleAsync(Guid committeeId, CancellationToken ct = default);
    }
}

using Monitoring.Application.Dtos;

namespace Monitoring.Application.Interfaces
{
    public interface IMonitoringRepository
    {
        Task<DashboardStatsDto> GetCommitteeStatisticsAsync();
        Task<List<MemberRiskDto>> GetOverloadedMembersAsync(int count);
        Task<List<ComplianceReportDto>> GetNonCompliantCommitteesAsync();
    }
}

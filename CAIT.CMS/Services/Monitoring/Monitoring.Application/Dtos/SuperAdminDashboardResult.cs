namespace Monitoring.Application.Dtos
{
    public record SuperAdminDashboardResult(DashboardStatsDto Stats, List<MemberRiskDto> RiskList);
}

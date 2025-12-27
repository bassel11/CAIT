namespace Monitoring.Application.Dtos
{
    public record DashboardStatsDto(int TotalCommittees, int ActiveCount, int NonCompliantCount, double AvgAttendance);
}

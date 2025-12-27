namespace Monitoring.Application.Dtos
{
    public record MemberRiskDto(string MemberName, string Department, int TotalCommittees, int PendingTasks);
}

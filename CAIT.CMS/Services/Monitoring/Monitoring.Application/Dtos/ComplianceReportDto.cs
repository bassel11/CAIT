namespace Monitoring.Application.Dtos
{
    public record ComplianceReportDto(string CommitteeName, string? Reason, DateTime? LastActivity);
}

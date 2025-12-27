using BuildingBlocks.Shared.CQRS;
using Monitoring.Application.Dtos;

namespace Monitoring.Application.Features.Monitoring.Queries.GetComplianceReport
{
    public record GetComplianceReportQuery : IQuery<List<ComplianceReportDto>>;
}

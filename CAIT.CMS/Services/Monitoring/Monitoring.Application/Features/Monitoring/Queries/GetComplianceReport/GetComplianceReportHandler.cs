using MediatR;
using Monitoring.Application.Dtos;
using Monitoring.Application.Interfaces;

namespace Monitoring.Application.Features.Monitoring.Queries.GetComplianceReport
{
    public class GetComplianceReportHandler : IRequestHandler<GetComplianceReportQuery, List<ComplianceReportDto>>
    {
        private readonly IMonitoringRepository _repository;

        public GetComplianceReportHandler(IMonitoringRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ComplianceReportDto>> Handle(GetComplianceReportQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetNonCompliantCommitteesAsync();
        }
    }
}

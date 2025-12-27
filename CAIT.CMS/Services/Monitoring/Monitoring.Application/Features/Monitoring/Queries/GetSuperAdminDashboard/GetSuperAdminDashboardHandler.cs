using BuildingBlocks.Shared.CQRS;
using Monitoring.Application.Dtos;
using Monitoring.Application.Interfaces;

namespace Monitoring.Application.Features.Monitoring.Queries.GetSuperAdminDashboard
{
    public class GetSuperAdminDashboardHandler : IQueryHandler<GetSuperAdminDashboardQuery, SuperAdminDashboardResult>
    {
        private readonly IMonitoringRepository _repository;

        public GetSuperAdminDashboardHandler(IMonitoringRepository repository)
        {
            _repository = repository;
        }

        public async Task<SuperAdminDashboardResult> Handle(GetSuperAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var stats = await _repository.GetCommitteeStatisticsAsync();
            var risks = await _repository.GetOverloadedMembersAsync(10); // Top 10

            return new SuperAdminDashboardResult(stats, risks);
        }
    }
}

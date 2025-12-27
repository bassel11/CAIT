using BuildingBlocks.Shared.CQRS;
using Monitoring.Application.Dtos;

namespace Monitoring.Application.Features.Monitoring.Queries.GetSuperAdminDashboard
{
    public record GetSuperAdminDashboardQuery : IQuery<SuperAdminDashboardResult>;
}

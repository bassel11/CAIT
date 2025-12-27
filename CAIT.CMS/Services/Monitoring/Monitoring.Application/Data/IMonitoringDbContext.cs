using Microsoft.EntityFrameworkCore;
using Monitoring.Core.Entities;

namespace Monitoring.Application.Data
{
    public interface IMonitoringDbContext
    {
        DbSet<CommitteeSummary> CommitteeSummaries { get; }
        DbSet<MemberWorkload> MemberWorkloads { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

using DecisionCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace DecisionApplication.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Decision> Decisions { get; }
        DbSet<Vote> Votes { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

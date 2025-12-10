using Audit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audit.Infrastructure.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(b =>
            {
                b.HasIndex(x => x.EventId);
                b.HasIndex(x => new { x.ServiceName, x.EntityName });
                b.HasIndex(x => x.Timestamp);
                b.HasIndex(x => x.PrimaryKey);
            });
        }
    }
}

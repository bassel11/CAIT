using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Monitoring.Application.Data;
using Monitoring.Core.Entities;
using System.Reflection;

namespace Monitoring.Infrastructure.Data
{
    public class MonitoringDbContext : DbContext, IMonitoringDbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options)
            : base(options) { }

        // Read Models
        public DbSet<CommitteeSummary> CommitteeSummaries => Set<CommitteeSummary>();
        public DbSet<MemberWorkload> MemberWorkloads => Set<MemberWorkload>();

        // MassTransit Outbox / Inbox
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<OutboxState> OutboxStates => Set<OutboxState>();
        public DbSet<InboxState> InboxStates => Set<InboxState>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply IEntityTypeConfiguration<>
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Domain Indexes
            builder.Entity<CommitteeSummary>().HasIndex(c => c.Status);
            builder.Entity<CommitteeSummary>().HasIndex(c => c.LastActivityDate);
            builder.Entity<MemberWorkload>().HasKey(m => m.MemberId);

            // MassTransit Transactional Outbox
            builder.AddTransactionalOutboxEntities();
        }
    }
}

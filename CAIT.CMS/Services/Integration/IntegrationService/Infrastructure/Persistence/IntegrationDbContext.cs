using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;


namespace IntegrationService.Infrastructure.Persistence
{
    public class IntegrationDbContext : DbContext
    {
        public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options) : base(options)
        {
        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<OutboxState> OutboxStates { get; set; }
        public DbSet<InboxState> InboxStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // هذه الأسطر السحرية هي التي تنشئ جداول MassTransit تلقائياً
            // (InboxState, OutboxState, OutboxMessage, etc.)
            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}

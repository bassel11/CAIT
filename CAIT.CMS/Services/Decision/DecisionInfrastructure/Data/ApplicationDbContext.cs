using DecisionApplication.Data;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using System.Reflection;

namespace DecisionInfrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Decision> Decisions => Set<Decision>();
        public DbSet<Vote> Votes => Set<Vote>();

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<OutboxState> OutboxStates { get; set; }
        public DbSet<InboxState> InboxStates { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);

            builder.AddTransactionalOutboxEntities();
            //builder.AddInboxStateEntity();
            //builder.AddOutboxMessageEntity();
            //builder.AddOutboxStateEntity();
        }
    }
}

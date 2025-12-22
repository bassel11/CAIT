using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using System.Reflection;
using TaskApplication.Data;

namespace TaskInfrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
        public DbSet<TaskNote> TaskNotes => Set<TaskNote>();
        public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
        public DbSet<TaskHistory> TaskHistories => Set<TaskHistory>();
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

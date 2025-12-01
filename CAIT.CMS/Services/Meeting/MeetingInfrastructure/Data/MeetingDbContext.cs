using MeetingCore.Entities;
using MeetingInfrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace MeetingInfrastructure.Data
{
    public class MeetingDbContext : DbContext
    {
        public MeetingDbContext(DbContextOptions<MeetingDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<AgendaItem> AgendaItems => Set<AgendaItem>();
        public DbSet<Attendance> Attendance => Set<Attendance>();
        public DbSet<MinutesOfMeeting> Minutes => Set<MinutesOfMeeting>();
        public DbSet<MinutesVersion> MinutesVersions => Set<MinutesVersion>();
        public DbSet<MeetingDecision> Decisions => Set<MeetingDecision>();
        public DbSet<MeetingVote> Votes => Set<MeetingVote>();
        public DbSet<AIGeneratedContent> AIGeneratedContents => Set<AIGeneratedContent>();
        public DbSet<MeetingIntegrationLog> IntegrationLogs => Set<MeetingIntegrationLog>();
        public DbSet<MeetingNotification> Notifications => Set<MeetingNotification>();
        public DbSet<MoMAttachment> MoMAttachments => Set<MoMAttachment>();

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations from this assembly (if you split into classes)
            modelBuilder.ApplyConfiguration(new MeetingConfiguration());
            modelBuilder.ApplyConfiguration(new AgendaItemConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new MinutesOfMeetingConfiguration());
            modelBuilder.ApplyConfiguration(new MinutesVersionConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingDecisionConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingVoteConfiguration());
            modelBuilder.ApplyConfiguration(new AIGeneratedContentConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingIntegrationLogConfiguration());
            modelBuilder.ApplyConfiguration(new MeetingNotificationConfiguration());
            modelBuilder.ApplyConfiguration(new MoMAttachmentConfiguration());

            base.OnModelCreating(modelBuilder);

        }

        // Optional: override SaveChanges to set CreatedAt/UpdatedAt automatically
        public override int SaveChanges()
        {
            //ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Meeting
                    || e.Entity is AgendaItem
                    || e.Entity is Attendance
                    //|| e.Entity is MinutesOfMeeting
                    || e.Entity is MinutesVersion
                    || e.Entity is MeetingDecision
                    || e.Entity is MeetingVote
                    || e.Entity is AIGeneratedContent
                    || e.Entity is MeetingIntegrationLog
                    || e.Entity is MeetingNotification
                    //|| e.Entity is MoMAttachment
                    );

            var now = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Property("CreatedAt") != null)
                        entry.Property("CreatedAt").CurrentValue = now;
                }

                if (entry.Entity is Meeting m)
                {
                    if (entry.State == EntityState.Added)
                        m.CreatedAt = now;
                    if (entry.State == EntityState.Modified)
                        m.UpdatedAt = now;
                }
            }
        }
    }
}

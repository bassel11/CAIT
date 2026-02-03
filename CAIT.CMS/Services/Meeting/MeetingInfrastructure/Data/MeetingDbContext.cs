using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MeetingApplication.Data;
using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeetingInfrastructure.Data
{
    public class MeetingDbContext : DbContext, IMeetingDbContext
    {
        public MeetingDbContext(DbContextOptions<MeetingDbContext> options) : base(options) { }

        // DbSets

        // ======================= Core Aggregates =======================
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<AgendaItem> AgendaItems => Set<AgendaItem>();
        public DbSet<AgendaItemAttachment> AgendaItemAttachments => Set<AgendaItemAttachment>();
        public DbSet<AgendaTemplate> AgendaTemplates => Set<AgendaTemplate>();
        public DbSet<AgendaTemplateItem> AgendaTemplateItems => Set<AgendaTemplateItem>();
        public DbSet<Attendance> Attendances => Set<Attendance>();

        // ======================= Minutes of Meeting & Related =======================
        public DbSet<MinutesOfMeeting> Minutes => Set<MinutesOfMeeting>();
        public DbSet<MinutesVersion> MinutesVersions => Set<MinutesVersion>();
        public DbSet<MoMAttachment> MoMAttachments => Set<MoMAttachment>();


        // Snapshot

        // ✅ الجداول الجديدة للقطات (Snapshots)
        public DbSet<MoMAttendance> MoMAttendances => Set<MoMAttendance>();
        public DbSet<MoMDiscussion> MoMDiscussions => Set<MoMDiscussion>();

        // New Draft Entities
        public DbSet<MoMDecisionDraft> MoMDecisionDrafts => Set<MoMDecisionDraft>();
        public DbSet<MoMActionItemDraft> MoMActionItemDrafts => Set<MoMActionItemDraft>();

        // ======================= AI & Integration =======================
        public DbSet<AIGeneratedContent> AIGeneratedContents => Set<AIGeneratedContent>();
        // public DbSet<MeetingIntegrationLog> IntegrationLogs => Set<MeetingIntegrationLog>();


        // ======================= Audit & Outbox =======================
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        // MassTransit Tables
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<OutboxState> OutboxStates { get; set; }



        //public DbSet<MeetingDecision> Decisions => Set<MeetingDecision>();
        //public DbSet<MeetingVote> Votes => Set<MeetingVote>();
        //public DbSet<MeetingNotification> Notifications => Set<MeetingNotification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddTransactionalOutboxEntities();
            //builder.AddInboxStateEntity();
            //builder.AddOutboxMessageEntity();
            //builder.AddOutboxStateEntity();
        }

        // Optional: override SaveChanges to set CreatedAt/UpdatedAt automatically
        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Meeting
                    || e.Entity is AgendaItem
                    || e.Entity is Attendance
                    || e.Entity is AgendaTemplate // ✅ إضافة القوالب للتدقيق
                    || e.Entity is AgendaItemAttachment
                    || e.Entity is MinutesOfMeeting
                    || e.Entity is MinutesVersion
                    || e.Entity is AIGeneratedContent
                    || e.Entity is MoMAttachment
                    || e.Entity is MoMAttendance
                    || e.Entity is MoMDiscussion
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
                        m.LastTimeModified = now;
                }
            }
        }
    }
}

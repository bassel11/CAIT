using CommitteeCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommitteeInfrastructure.Data
{
    public class CommitteeContext : DbContext
    {
        public CommitteeContext()
        {
        }
        public CommitteeContext(DbContextOptions<CommitteeContext> options) : base(options)
        {

        }

        // تمت اضافته من اجل رؤية SQL Query الحاصلة من خلال Console

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .EnableSensitiveDataLogging() // لعرض قيم المتغيرات
        //        .LogTo(Console.WriteLine, LogLevel.Information); // أو ILogger
        //}

        public DbSet<Committee> Committees { get; set; }
        public DbSet<CommitteeMember> CommitteeMembers { get; set; }
        public DbSet<CommitteeMemberRole> CommitteeMemberRoles { get; set; }

        public DbSet<CommitteeStatus> CommitteeStatuses { get; set; }
        public DbSet<CommitteeStatusHistory> CommitteeStatusHistories { get; set; }

        public DbSet<CommitteeAuditLog> CommitteeAuditLogs { get; set; }

        public DbSet<CommitteeDocument> CommitteeDocuments { get; set; }
        public DbSet<CommitteeDecision> CommitteeDecisions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ---------------------------------------
            // Committee
            // ---------------------------------------

            modelBuilder.Entity<Committee>()
                .Property(c => c.Budget)
                .HasPrecision(18, 2); // <-- يمكنك تعديل الدقة والقياس حسب حاجتك

            modelBuilder.Entity<Committee>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Committee>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Committee (FK to CommitteeStatus)
            modelBuilder.Entity<Committee>()
                .HasOne(c => c.Status)
                .WithMany(s => s.Committees)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);


            // CommitteeStatus (Lookup)
            modelBuilder.Entity<CommitteeStatus>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<CommitteeStatus>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CommitteeStatus>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");



            // ---------------------------------------
            // CommitteeMember
            // ---------------------------------------

            modelBuilder.Entity<CommitteeMember>()
            .HasIndex(cm => new { cm.CommitteeId, cm.UserId })
            .IsUnique();

            modelBuilder.Entity<CommitteeMember>()
                .Property(m => m.Affiliation)
                .HasMaxLength(200);

            modelBuilder.Entity<CommitteeMember>()
                .HasOne(m => m.Committee)
                .WithMany(c => c.CommitteeMembers)
                .HasForeignKey(m => m.CommitteeId);

            modelBuilder.Entity<CommitteeMember>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // ---------------------------------------
            // CommitteeMemberRole
            // ---------------------------------------
            modelBuilder.Entity<CommitteeMemberRole>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<CommitteeMemberRole>()
                .HasOne(r => r.CommitteeMember)
                .WithMany(m => m.CommitteeMemberRoles)
                .HasForeignKey(r => r.CommitteeMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CommitteeMemberRole>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<CommitteeMemberRole>()
            .HasIndex(r => new { r.CommitteeMemberId, r.RoleId })
            .IsUnique()
            .HasDatabaseName("UX_CommitteeMember_Role"); // اسم مخصص للمفتاح الفريد


            // ---------------------------------------
            // CommitteeStatusHistory
            // ---------------------------------------
            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasKey(h => h.Id);

            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasOne(h => h.Committee)
                .WithMany(c => c.CommitteeStatusHistories)
                .HasForeignKey(h => h.CommitteeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship OldStatus
            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasOne(h => h.OldStatus)
                .WithMany(s => s.OldStatusHistories)
                .HasForeignKey(h => h.OldStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship NewStatus
            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasOne(h => h.NewStatus)
                .WithMany(s => s.NewStatusHistories)
                .HasForeignKey(h => h.NewStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommitteeStatusHistory>()
                .Property(h => h.DecisionText)
                .IsRequired();

            // 🔹 Index على CommitteeId لتحسين أداء الاستعلامات
            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasIndex(h => h.CommitteeId)
                .HasDatabaseName("IX_CommitteeStatusHistories_CommitteeId");

            // 🔹 Index مركب على CommitteeId + ChangedAt لتحسين Pagination مع Filter
            modelBuilder.Entity<CommitteeStatusHistory>()
                .HasIndex(h => new { h.CommitteeId, h.ChangedAt })
                .HasDatabaseName("IX_CommitteeStatusHistories_CommitteeId_ChangedAt");

            // ---------------------------------------
            // CommitteeAuditLog
            // ---------------------------------------
            modelBuilder.Entity<CommitteeAuditLog>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<CommitteeAuditLog>()
                .HasOne(a => a.Committee)
                .WithMany(c => c.CommitteeAuditLogs)
                .HasForeignKey(a => a.CommitteeId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================
            // SEEDING: CommitteeStatus
            // ================================
            modelBuilder.Entity<CommitteeStatus>().HasData(
            new CommitteeStatus { Id = 1, Name = "Draft", CreatedAt = new DateTime(2025, 11, 1) },
            new CommitteeStatus { Id = 2, Name = "Active", CreatedAt = new DateTime(2025, 11, 1) },
            new CommitteeStatus { Id = 3, Name = "Suspended", CreatedAt = new DateTime(2025, 11, 1) },
            new CommitteeStatus { Id = 4, Name = "Completed", CreatedAt = new DateTime(2025, 11, 1) },
            new CommitteeStatus { Id = 5, Name = "Dissolved", CreatedAt = new DateTime(2025, 11, 1) },
            new CommitteeStatus { Id = 6, Name = "Archived", CreatedAt = new DateTime(2025, 11, 1) }
);

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }

}
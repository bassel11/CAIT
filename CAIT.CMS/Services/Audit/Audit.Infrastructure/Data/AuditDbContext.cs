using Audit.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;


namespace Audit.Infrastructure.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. إعدادات MassTransit (لضمان الموثوقية وعدم التكرار)
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            // 2. إعدادات جدول AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");

                // --- الفهارس (Indexes) بناءً على متطلبات CMS.docx ---

                //  "Logs filterable by: committee"
                // ضروري لتقارير اللجان ولتقييد صلاحيات مدراء اللجان
                entity.HasIndex(e => e.CommitteeId)
                      .HasDatabaseName("IX_AuditLogs_CommitteeId");

                //  "Logs filterable by: user"
                // لتتبع نشاط مستخدم معين (User Accountability) [cite: 5]
                entity.HasIndex(e => e.UserId)
                      .HasDatabaseName("IX_AuditLogs_UserId");

                //  "Logs filterable by: activity type"
                // للبحث عن أخطاء الدخول أو التعديلات الحرجة
                entity.HasIndex(e => e.ActionType)
                      .HasDatabaseName("IX_AuditLogs_ActionType");

                //  "Logs filterable by: time range"
                // فهرس مركب للترتيب الزمني وهو أساسي لعرض الـ Chain بشكل صحيح
                entity.HasIndex(e => new { e.Timestamp, e.ReceivedAt })
                      .HasDatabaseName("IX_AuditLogs_TimeOrder");

                // فهرس للبحث عن سجلات كيان معين (مثلاً تاريخ قرار رقم 99)
                entity.HasIndex(e => e.PrimaryKey);

                // --- قيود البيانات (Constraints) ---

                // تحسين الأداء بتقليل حجم النصوص (بدلاً من nvarchar(max))
                entity.Property(e => e.ServiceName).HasMaxLength(100);
                entity.Property(e => e.EntityName).HasMaxLength(100);
                entity.Property(e => e.ActionType).HasMaxLength(50);
                entity.Property(e => e.UserId).HasMaxLength(100);
                entity.Property(e => e.CommitteeId).HasMaxLength(100);
                entity.Property(e => e.PrimaryKey).HasMaxLength(100);

                // --- الأمان (Tamper-Proofing)  ---
                // الهاش إلزامي وحجمه ثابت (SHA256 ينتج دائماً 64 حرف Hex)
                entity.Property(e => e.Hash)
                      .IsRequired()
                      .HasMaxLength(64)
                      .IsFixedLength();

                entity.Property(e => e.PreviousHash)
                      .HasMaxLength(64)
                      .IsFixedLength();
            });
        }
    }
}

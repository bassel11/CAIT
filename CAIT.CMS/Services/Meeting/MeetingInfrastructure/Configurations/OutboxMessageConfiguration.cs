using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> b)
        {
            // المفتاح الأساسي
            b.HasKey(x => x.Id);

            // خصائص مطلوبة
            b.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(x => x.Payload)
                .IsRequired();

            b.Property(x => x.OccurredAt)
                .IsRequired();

            // خصائص القفل
            b.Property(x => x.LockedBy)
                .HasMaxLength(100);

            b.Property(x => x.LockedAt);

            // خصائص المعالجة
            b.Property(x => x.Processed)
                .HasDefaultValue(false);

            b.Property(x => x.Attempts)
                .HasDefaultValue(0);

            b.Property(x => x.LastError)
                .HasMaxLength(2000);

            // Concurrency Token (RowVersion)
            b.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // فهرس لتحسين الاستعلامات
            b.HasIndex(x => new { x.Processed, x.Attempts, x.LockedBy, x.OccurredAt });
        }
    }
}
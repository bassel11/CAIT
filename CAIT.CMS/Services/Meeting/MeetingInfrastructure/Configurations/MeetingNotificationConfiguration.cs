using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingNotificationConfiguration : IEntityTypeConfiguration<MeetingNotification>
    {
        public void Configure(EntityTypeBuilder<MeetingNotification> b)
        {
            b.ToTable("MeetingNotifications");
            b.HasKey(x => x.Id);

            b.Property(x => x.NotificationType)
                .HasConversion<string>()
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.PayloadJson).HasColumnType("nvarchar(max)").IsRequired();
            b.Property(x => x.Processed).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.ProcessedAt);

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => new { x.Processed, x.CreatedAt });
        }
    }
}

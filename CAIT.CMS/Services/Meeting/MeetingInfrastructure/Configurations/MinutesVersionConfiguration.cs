using MeetingCore.Entities;
using MeetingCore.ValueObjects.AttendanceVO; // For UserId
using MeetingCore.ValueObjects.MinutesVersionVO;
using MeetingCore.ValueObjects.MinutesVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MinutesVersionConfiguration : IEntityTypeConfiguration<MinutesVersion>
    {
        public void Configure(EntityTypeBuilder<MinutesVersion> builder)
        {
            builder.ToTable("MinutesVersions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MinutesVersionId.Of(value));

            builder.Property(x => x.MoMId)
                .HasConversion(id => id.Value, value => MoMId.Of(value))
                .IsRequired();

            // محتوى النسخة قد يكون كبيراً
            builder.Property(x => x.Content)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.VersionNumber)
                .IsRequired();

            // UserId Value Object Conversion
            builder.Property(x => x.ModifiedBy)
                .HasConversion(id => id.Value, value => UserId.Of(value))
                .IsRequired();

            builder.Property(x => x.ModifiedAt)
                .IsRequired();

            // فهرس مركب لضمان عدم تكرار رقم النسخة لنفس المحضر
            builder.HasIndex(x => new { x.MoMId, x.VersionNumber })
                .IsUnique();
        }
    }
}
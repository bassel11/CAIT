using MeetingCore.Entities;
using MeetingCore.ValueObjects.AttendanceVO; // For UserId
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMAttachmentVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MoMAttachmentConfiguration : IEntityTypeConfiguration<MoMAttachment>
    {
        public void Configure(EntityTypeBuilder<MoMAttachment> builder)
        {
            builder.ToTable("MoMAttachments");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MoMAttachmentId.Of(value));

            builder.Property(x => x.MoMId)
                .HasConversion(id => id.Value, value => MoMId.Of(value))
                .IsRequired();

            builder.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.SizeInBytes)
                .IsRequired();

            builder.Property(x => x.StoragePath)
                .HasMaxLength(1000)
                .IsRequired();

            // UserId Value Object Conversion
            builder.Property(x => x.UploadedBy)
                .HasConversion(id => id.Value, value => UserId.Of(value))
                .IsRequired();

            builder.Property(x => x.UploadedAt)
                .IsRequired();

            builder.HasIndex(x => x.MoMId);
        }
    }
}
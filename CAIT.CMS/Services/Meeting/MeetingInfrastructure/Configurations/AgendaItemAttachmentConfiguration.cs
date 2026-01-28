using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaItemAttachmentVO;
using MeetingCore.ValueObjects.AgendaItemVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AgendaItemAttachmentConfiguration : IEntityTypeConfiguration<AgendaItemAttachment>
    {
        public void Configure(EntityTypeBuilder<AgendaItemAttachment> builder)
        {
            builder.ToTable("AgendaItemAttachments");

            // 1. Primary Key (Strong ID)
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, val => AgendaItemAttachmentId.Of(val));

            // 2. Foreign Key (Strong ID)
            builder.Property(x => x.AgendaItemId)
                .HasConversion(id => id.Value, val => AgendaItemId.Of(val))
                .IsRequired();

            // 3. ✅ استخدام ComplexProperty كما طلبت (Modern Approach)
            // هذا يغنينا عن OwnsOne ويجعل الأعمدة مسطحة مباشرة في الجدول
            builder.ComplexProperty(x => x.FileDetails, fileBuilder =>
            {
                fileBuilder.Property(p => p.FileName)
                    .HasColumnName("FileName")
                    .HasMaxLength(255)
                    .IsRequired();

                fileBuilder.Property(p => p.FileUrl)
                    .HasColumnName("FileUrl")
                    .HasMaxLength(2048)
                    .IsRequired();

                fileBuilder.Property(p => p.ContentType)
                    .HasColumnName("ContentType")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Property(x => x.UploadedAt).IsRequired();

            // 4. Index for Performance
            builder.HasIndex(x => x.AgendaItemId);
        }
    }
}

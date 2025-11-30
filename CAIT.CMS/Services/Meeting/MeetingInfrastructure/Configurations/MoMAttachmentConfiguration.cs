using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MoMAttachmentConfiguration : IEntityTypeConfiguration<MoMAttachment>
    {
        public void Configure(EntityTypeBuilder<MoMAttachment> b)
        {
            b.ToTable("MoMAttachments");
            b.HasKey(x => x.Id);
            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.HasIndex(x => x.MoMId);

            b.HasOne(x => x.MoM)
                .WithMany(m => m.MoMAttachments)
                .HasForeignKey(x => x.MoMId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

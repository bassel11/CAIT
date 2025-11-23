using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MinutesVersionConfiguration : IEntityTypeConfiguration<MinutesVersion>
    {
        public void Configure(EntityTypeBuilder<MinutesVersion> b)
        {
            b.ToTable("MinutesVersions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
            b.Property(x => x.VersionNumber).IsRequired();
            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.MoMId);
            b.HasIndex(x => new { x.MoMId, x.VersionNumber }).IsUnique(false);

            b.HasOne(x => x.MoM)
                .WithMany(m => m.Versions)
                .HasForeignKey(x => x.MoMId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

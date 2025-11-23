using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingIntegrationLogConfiguration : IEntityTypeConfiguration<MeetingIntegrationLog>
    {
        public void Configure(EntityTypeBuilder<MeetingIntegrationLog> b)
        {
            b.ToTable("MeetingIntegrationLogs");
            b.HasKey(x => x.Id);

            b.Property(x => x.IntegrationType)
                .HasConversion<string>()
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.Success).IsRequired();
            b.Property(x => x.ExternalId).HasMaxLength(200);
            b.Property(x => x.ErrorMessage).HasMaxLength(2000);
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => x.IntegrationType);
        }
    }
}

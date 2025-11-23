using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MinutesOfMeetingConfiguration : IEntityTypeConfiguration<MinutesOfMeeting>
    {
        public void Configure(EntityTypeBuilder<MinutesOfMeeting> b)
        {
            b.ToTable("MinutesOfMeetings");
            b.HasKey(x => x.Id);

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.AttendanceSummary).HasMaxLength(4000);
            b.Property(x => x.AgendaSummary).HasMaxLength(4000);
            b.Property(x => x.DecisionsSummary).HasMaxLength(4000);

            b.Property(x => x.ActionItemsJson).HasColumnType("nvarchar(max)");

            b.Property(x => x.VersionNumber).IsRequired();

            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => new { x.MeetingId, x.VersionNumber });
        }
    }
}

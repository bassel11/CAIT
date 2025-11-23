using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> b)
        {
            b.ToTable("AttendanceRecords");
            b.HasKey(x => x.Id);

            b.Property(x => x.MemberId).IsRequired();

            b.Property(x => x.RSVP)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.AttendanceStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.Timestamp);

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => x.MemberId);
            b.HasIndex(x => new { x.MeetingId, x.MemberId }).IsUnique(false);
        }
    }
}
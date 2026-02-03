using MeetingCore.Entities;
using MeetingCore.Enums.MoMEnums;
using MeetingCore.ValueObjects.MeetingVO;
using MeetingCore.ValueObjects.MinutesVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MinutesOfMeetingConfiguration : IEntityTypeConfiguration<MinutesOfMeeting>
    {
        public void Configure(EntityTypeBuilder<MinutesOfMeeting> builder)
        {
            builder.ToTable("MinutesOfMeetings");

            // 1. Keys & IDs
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MoMId.Of(value));

            builder.Property(x => x.MeetingId)
                .HasConversion(id => id.Value, value => MeetingId.Of(value))
                .IsRequired();

            // 2. Properties
            builder.Property(x => x.Status)
                .HasConversion(
                    s => s.ToString(),
                    v => (MoMStatus)Enum.Parse(typeof(MoMStatus), v))
                .HasMaxLength(50)
                .IsRequired();

            // المحتوى HTML قد يكون كبيراً جداً
            builder.Property(x => x.FullContentHtml)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.VersionNumber)
                .IsRequired();

            // Audit & Approvals
            builder.Property(x => x.ApprovedBy); // Nullable Guid
            builder.Property(x => x.ApprovedAt); // Nullable DateTime

            builder.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CreatedAt).IsRequired();

            builder.Property(x => x.RowVersion).IsRowVersion();

            // 3. Relationships & Backing Fields (CRITICAL)

            // Decisions Drafts
            builder.HasMany(x => x.Decisions)
                .WithOne()
                .HasForeignKey("MoMId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.Decisions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Action Items Drafts
            builder.HasMany(x => x.ActionItems)
                .WithOne()
                .HasForeignKey("MoMId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.ActionItems))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Attachments
            builder.HasMany(x => x.Attachments)
                .WithOne()
                .HasForeignKey("MoMId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.Attachments))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Versions
            builder.HasMany(x => x.Versions)
                .WithOne()
                .HasForeignKey("MoMId") // أو x.MoMId إذا كان معرفاً في Version
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.Versions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // ✅ E. Attendance Snapshot (الجديد)
            builder.HasMany(x => x.AttendanceSnapshot)
                .WithOne()
                .HasForeignKey(x => x.MoMId) // Explicit FK in MoMAttendance
                .OnDelete(DeleteBehavior.Cascade);

            // استخدام الـ Field للتأكد من أن EF Core يستخدم _attendanceSnapshot
            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.AttendanceSnapshot))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // ✅ F. Discussions (الجديد)
            builder.HasMany(x => x.Discussions)
                .WithOne()
                .HasForeignKey(x => x.MoMId) // Explicit FK in MoMDiscussion
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(MinutesOfMeeting.Discussions))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // 4. Indexes
            builder.HasIndex(x => x.MeetingId).IsUnique(); // محضر واحد لكل اجتماع
            builder.HasIndex(x => x.Status);
        }
    }
}
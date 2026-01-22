using MeetingCore.Entities;
using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            // =========================================================
            // 1. Primary Key
            // =========================================================
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => AttendanceId.Of(value));

            // =========================================================
            // 2. Foreign Keys & References
            // =========================================================

            // MeetingId (Strongly Typed Wrapper)
            builder.Property(x => x.MeetingId)
                .HasConversion(
                    id => id.Value,
                    value => MeetingId.Of(value))
                .IsRequired();

            // UserId (Strongly Typed Wrapper)
            builder.Property(x => x.UserId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.Of(value))
                .IsRequired();

            // =========================================================
            // 3. Enums (Store as String for Readability)
            // =========================================================
            // تخزين الـ Enums كنصوص يجعل قاعدة البيانات قابلة للقراءة
            // ويحمي البيانات من التلف في حال تغير ترتيب الـ Enum في الكود

            builder.Property(x => x.Role)
                .HasConversion(
                    r => r.ToString(),
                    v => (AttendanceRole)Enum.Parse(typeof(AttendanceRole), v))
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.VotingRight)
                .HasConversion(
                    r => r.ToString(),
                    v => (VotingRight)Enum.Parse(typeof(VotingRight), v))
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.RSVP)
                .HasConversion(
                    r => r.ToString(),
                    v => (RSVPStatus)Enum.Parse(typeof(RSVPStatus), v))
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.AttendanceStatus)
                .HasConversion(
                    r => r.ToString(),
                    v => (AttendanceStatus)Enum.Parse(typeof(AttendanceStatus), v))
                .HasMaxLength(50)
                .IsRequired();

            // =========================================================
            // 4. Primitives
            // =========================================================
            builder.Property(x => x.CheckInTime); // Nullable DateTime, no config needed

            // =========================================================
            // 5. Indexes (Business Rules Enforced by DB)
            // =========================================================

            // أهم قاعدة: لا يمكن إضافة نفس المستخدم لنفس الاجتماع مرتين
            builder.HasIndex(x => new { x.MeetingId, x.UserId })
                .IsUnique();

            // فهرس للبحث السريع عن كل اجتماعات مستخدم معين
            builder.HasIndex(x => x.UserId);
        }
    }
}
using MeetingCore.Entities;
using MeetingCore.Enums.AttendanceEnums;
using MeetingCore.ValueObjects.AttendanceVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMAttendanceVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MoMAttendanceConfiguration : IEntityTypeConfiguration<MoMAttendance>
    {
        public void Configure(EntityTypeBuilder<MoMAttendance> builder)
        {
            builder.ToTable("MoMAttendances");

            // 1. Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => MoMAttendanceId.Of(value));

            // 2. Foreign Key (MoMId)
            builder.Property(x => x.MoMId)
                .HasConversion(
                    id => id.Value,
                    value => MoMId.Of(value))
                .IsRequired();

            // 3. UserId Conversion
            builder.Property(x => x.UserId)
                .HasConversion(
                    id => id.Value,
                    value => UserId.Of(value))
                .IsRequired();

            // 4. Properties
            builder.Property(x => x.MemberName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(100);

            // 5. Enum Conversion (Storing as String for readability or Int for performance)
            // يفضل String في الحالات التي نحتاج لقراءة الداتابيز مباشرة
            builder.Property(x => x.Status)
                .HasConversion(
                    s => s.ToString(),
                    v => (AttendanceStatus)Enum.Parse(typeof(AttendanceStatus), v))
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.IsPresent)
                .IsRequired();

            builder.Property(x => x.AbsenceReason)
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            // 6. Audit Fields (Inherited from Entity<T>)
            // نفترض أن Entity<T> تحتوي على CreatedBy/CreatedAt كـ Shadow Properties أو صريحة
            // إذا كانت صريحة، يجب تهيئتها هنا، لكننا قمنا بذلك في DbContext
        }
    }
}

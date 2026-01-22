using MeetingCore.Entities;
using MeetingCore.Enums.MeetingEnums;
using MeetingCore.ValueObjects;
using MeetingCore.ValueObjects.MeetingVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.ToTable("Meetings");

            // =========================================================
            // 1. Keys & Strongly Typed IDs
            // =========================================================
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MeetingId.Of(value));

            // =========================================================
            // 2. Simple Value Objects
            // =========================================================
            builder.Property(x => x.CommitteeId)
                .HasConversion(id => id.Value, value => CommitteeId.Of(value))
                .IsRequired();

            builder.Property(x => x.Title)
                .HasConversion(t => t.Value, v => MeetingTitle.Of(v))
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.TimeZone)
                .HasConversion(t => t.Value, v => TimeZoneId.Of(v))
                .HasMaxLength(100)
                .IsRequired();

            // =========================================================
            // 3. Complex Value Objects
            // =========================================================

            // Location
            builder.ComplexProperty(x => x.Location, locBuilder =>
            {
                locBuilder.Property(p => p.Type)
                    .HasColumnName("LocationType")
                    .HasConversion(v => v.ToString(), v => (LocationType)Enum.Parse(typeof(LocationType), v))
                    .HasMaxLength(50);

                locBuilder.Property(p => p.RoomName).HasColumnName("LocationRoomName").HasMaxLength(200);
                locBuilder.Property(p => p.Address).HasColumnName("LocationAddress").HasMaxLength(500);
                locBuilder.Property(p => p.OnlineUrl).HasColumnName("LocationOnlineUrl").HasMaxLength(1000);
            });

            // Recurrence
            builder.ComplexProperty(x => x.Recurrence, recBuilder =>
            {
                recBuilder.Property(p => p.IsRecurring).HasColumnName("IsRecurring").HasDefaultValue(false);
                recBuilder.Property(p => p.Type)
                    .HasColumnName("RecurrenceType")
                    .HasConversion(v => v.ToString(), v => (RecurrenceType)Enum.Parse(typeof(RecurrenceType), v))
                    .HasMaxLength(50);
                recBuilder.Property(p => p.Rule).HasColumnName("RecurrenceRule").HasMaxLength(500);
            });

            // =========================================================
            // 4. Basic Properties & Enums
            // =========================================================
            builder.Property(x => x.Description).HasMaxLength(4000);
            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();

            builder.Property(x => x.Status)
                .HasConversion(s => s.ToString(), v => (MeetingStatus)Enum.Parse(typeof(MeetingStatus), v))
                .HasMaxLength(50).IsRequired();

            builder.Property(x => x.CancellationReason).HasMaxLength(2000);
            builder.Property(x => x.TeamsLink).HasMaxLength(1000);
            builder.Property(x => x.OutlookEventId).HasMaxLength(200);

            // =========================================================
            // 5. Auditing & Concurrency
            // =========================================================
            builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            builder.Property(x => x.RowVersion).IsRowVersion();

            // =========================================================
            // 6. Relationships & Encapsulation (Crucial for DDD)
            // =========================================================
            // هنا التعديل الجوهري لحل مشكلة Backing Field
            // =========================================================

            // 1. Agenda Items (1-to-Many)
            builder.HasMany(x => x.AgendaItems)
                .WithOne()
                .HasForeignKey("MeetingId")
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ استخدام Navigation Builder بدلاً من Metadata
            builder.Navigation(x => x.AgendaItems)
                .HasField("_agendaItems") // تحديد الحقل الخاص صراحة
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 2. Attendances (1-to-Many)
            builder.HasMany(x => x.Attendances)
                .WithOne()
                .HasForeignKey("MeetingId")
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ تحديد الحقل الخاص
            builder.Navigation(x => x.Attendances)
                .HasField("_attendances")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 3. AI Contents (1-to-Many)
            builder.HasMany(x => x.AIContents)
                .WithOne()
                .HasForeignKey("MeetingId")
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ تحديد الحقل الخاص (كان مسبباً للمشكلة سابقاً)
            builder.Navigation(x => x.AIContents)
                .HasField("_aiContents")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 4. Minutes (1-to-1)
            builder.HasOne(x => x.Minutes)
                .WithOne()
                .HasForeignKey<MinutesOfMeeting>(mom => mom.MeetingId) // ✅ استخدام Lambda Typed
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
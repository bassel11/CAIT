using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> b)
        {
            b.ToTable("Meetings");

            b.HasKey(x => x.Id);

            b.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.Description)
                .HasMaxLength(4000);

            b.Property(x => x.StartDate)
                .IsRequired();

            b.Property(x => x.EndDate)
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.RecurrenceType)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.RecurrenceRule)
                .HasMaxLength(1000);

            b.Property(x => x.TeamsLink)
                .HasMaxLength(1000);

            b.Property(x => x.OutlookEventId)
                .HasMaxLength(200);

            b.Property(x => x.CreatedBy).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.UpdatedAt);

            // RowVersion for concurrency
            b.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // Indexes
            b.HasIndex(x => x.CommitteeId);
            b.HasIndex(x => x.StartDate);
            b.HasIndex(x => x.Status);

            // OutlookEventId unique per tenant/meetings: optional uniqueness
            b.HasIndex(x => x.OutlookEventId).IsUnique(false);

            // Relations
            b.HasMany(x => x.AgendaItems)
                .WithOne(a => a.Meeting)
                .HasForeignKey(a => a.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Attendances)
                .WithOne(ar => ar.Meeting)
                .HasForeignKey(ar => ar.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Minutes)
                .WithOne(m => m.Meeting)
                .HasForeignKey<MinutesOfMeeting>(m => m.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Decisions)
                .WithOne(d => d.Meeting)
                .HasForeignKey(d => d.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.AIGeneratedContents)
                .WithOne(ai => ai.Meeting)
                .HasForeignKey(ai => ai.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.IntegrationLogs)
                .WithOne(l => l.Meeting)
                .HasForeignKey(l => l.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Notifications)
                .WithOne(n => n.Meeting)
                .HasForeignKey(n => n.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaItemVO;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMDiscussionVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    internal class MoMDiscussionConfiguration : IEntityTypeConfiguration<MoMDiscussion>
    {
        public void Configure(EntityTypeBuilder<MoMDiscussion> builder)
        {
            builder.ToTable("MoMDiscussions");

            // 1. Primary Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => MoMDiscussionId.Of(value));

            // 2. Foreign Key to MoM
            builder.Property(x => x.MoMId)
                .HasConversion(
                    id => id.Value,
                    value => MoMId.Of(value))
                .IsRequired();

            // 3. Optional Reference to Agenda Item
            builder.Property(x => x.OriginalAgendaItemId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?)null,
                    value => value.HasValue ? AgendaItemId.Of(value.Value) : null);

            // 4. Content
            builder.Property(x => x.TopicTitle)
                .IsRequired()
                .HasMaxLength(500);

            // محتوى النقاش قد يكون طويلاً (HTML)
            builder.Property(x => x.DiscussionContent)
                .IsRequired()
                .HasColumnType("nvarchar(max)");
        }
    }
}

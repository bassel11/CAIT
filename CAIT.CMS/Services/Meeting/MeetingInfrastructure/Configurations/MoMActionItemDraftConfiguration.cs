using MeetingCore.Entities;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMActionItemDraftVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MoMActionItemDraftConfiguration : IEntityTypeConfiguration<MoMActionItemDraft>
    {
        public void Configure(EntityTypeBuilder<MoMActionItemDraft> builder)
        {
            builder.ToTable("MoMActionItemDrafts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MoMActionItemDraftId.Of(value));

            builder.Property(x => x.MoMId)
                .HasConversion(id => id.Value, value => MoMId.Of(value))
                .IsRequired();

            builder.Property(x => x.TaskTitle)
                .HasMaxLength(500)
                .IsRequired();

            // AssigneeId is nullable Guid
            builder.Property(x => x.AssigneeId);

            builder.Property(x => x.DueDate);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>() // DraftStatus Enum
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.MoMId);
        }
    }
}
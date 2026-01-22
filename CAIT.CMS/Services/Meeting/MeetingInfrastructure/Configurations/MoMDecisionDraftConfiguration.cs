using MeetingCore.Entities;
using MeetingCore.ValueObjects.MinutesVO;
using MeetingCore.ValueObjects.MoMDecisionDraftVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MoMDecisionDraftConfiguration : IEntityTypeConfiguration<MoMDecisionDraft>
    {
        public void Configure(EntityTypeBuilder<MoMDecisionDraft> builder)
        {
            builder.ToTable("MoMDecisionDrafts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasConversion(id => id.Value, value => MoMDecisionDraftId.Of(value));

            builder.Property(x => x.MoMId)
                .HasConversion(id => id.Value, value => MoMId.Of(value))
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.Text)
                .HasMaxLength(4000) // أو nvarchar(max) إذا كانت القرارات طويلة جداً
                .IsRequired();

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
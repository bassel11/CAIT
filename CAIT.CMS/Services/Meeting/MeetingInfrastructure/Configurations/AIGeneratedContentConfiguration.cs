using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AIGeneratedContentConfiguration : IEntityTypeConfiguration<AIGeneratedContent>
    {
        public void Configure(EntityTypeBuilder<AIGeneratedContent> b)
        {
            b.ToTable("AIGeneratedContents");
            b.HasKey(x => x.Id);

            b.Property(x => x.ContentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.Prompt).HasMaxLength(4000).IsRequired();
            b.Property(x => x.GeneratedText).HasColumnType("nvarchar(max)").IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.CreatedBy).IsRequired();

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => x.ContentType);
        }
    }
}

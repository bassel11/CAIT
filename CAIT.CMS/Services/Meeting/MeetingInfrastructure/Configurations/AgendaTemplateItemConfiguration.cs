using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaTemplateVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AgendaTemplateItemConfiguration : IEntityTypeConfiguration<AgendaTemplateItem>
    {
        public void Configure(EntityTypeBuilder<AgendaTemplateItem> builder)
        {
            builder.ToTable("AgendaTemplateItems");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion(id => id.Value, val => AgendaTemplateItemId.Of(val));

            // FK Config
            builder.Property(x => x.AgendaTemplateId)
                   .HasConversion(id => id.Value, val => AgendaTemplateId.Of(val))
                   .IsRequired();

            builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.SortOrder).IsRequired();

            builder.HasIndex(x => x.AgendaTemplateId);
        }
    }
}

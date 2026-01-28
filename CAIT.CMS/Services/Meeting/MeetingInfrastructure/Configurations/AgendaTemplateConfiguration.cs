using MeetingCore.Entities;
using MeetingCore.ValueObjects.AgendaTemplateVO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AgendaTemplateConfiguration : IEntityTypeConfiguration<AgendaTemplate>
    {
        public void Configure(EntityTypeBuilder<AgendaTemplate> builder)
        {
            builder.ToTable("AgendaTemplates");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasConversion(id => id.Value, val => AgendaTemplateId.Of(val));

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.IsActive).HasDefaultValue(true);

            // Audit
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();

            // العلاقة 1-to-Many
            builder.HasMany(x => x.Items)
                   .WithOne()
                   .HasForeignKey(item => item.AgendaTemplateId) // FK الصريح
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Items)
                   .HasField("_items")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class AgendaItemConfiguration : IEntityTypeConfiguration<AgendaItem>
    {
        public void Configure(EntityTypeBuilder<AgendaItem> b)
        {
            b.ToTable("AgendaItems");
            b.HasKey(x => x.Id);

            b.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(x => x.Description)
                .HasMaxLength(4000);

            b.Property(x => x.SortOrder).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.MeetingId);
            b.HasIndex(x => new { x.MeetingId, x.SortOrder });
        }
    }
}

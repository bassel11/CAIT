using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DecisionInfrastructure.Data.Configurations
{
    public class DecisionConfiguration : IEntityTypeConfiguration<Decision>
    {
        public void Configure(EntityTypeBuilder<Decision> builder)
        {
            builder.HasKey(d => d.Id);

            // تحويل DecisionId إلى Guid
            builder.Property(d => d.Id)
                .HasConversion(id => id.Value, dbId => DecisionId.Of(dbId));

            // تحويل MeetingId إلى Guid
            builder.Property(d => d.MeetingId)
                .HasConversion(id => id.Value, dbId => MeetingId.Of(dbId));

            builder.Property(d => d.MoMId)
                   .HasConversion(
                       id => id != null ? id.Value : (Guid?)null,
                       dbId => dbId.HasValue ? MoMId.Of(dbId.Value) : null
                   )
                   .IsRequired(false); // يمكن

            // تحويل AgendaItemId إلى Guid (nullable)
            builder.Property(d => d.AgendaItemId)
                   .HasConversion(
                       id => id != null ? id.Value : (Guid?)null,
                       dbId => dbId.HasValue ? AgendaItemId.Of(dbId.Value) : null
                   )
                   .IsRequired(false);

            // تحويل DecisionTitle إلى string
            builder.OwnsOne(d => d.Title, titleBuilder =>
            {
                titleBuilder.Property(t => t.Value)
                    .HasColumnName("Title")
                    .IsRequired();
            });


            // Owned entity لكل نص عربي و إنجليزي
            builder.OwnsOne(d => d.Text, textBuilder =>
            {
                textBuilder.Property(t => t.Arabic)
                    .HasColumnName("TextArabic")
                    .HasMaxLength(1000)
                    .IsRequired();

                textBuilder.Property(t => t.English)
                    .HasColumnName("TextEnglish")
                    .HasMaxLength(1000)
                    .IsRequired();
            });

            // تحويل Enums إلى string
            builder.Property(d => d.Type)
                .HasConversion(
                    t => t.ToString(),
                    db => (DecisionType)Enum.Parse(typeof(DecisionType), db))
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion(
                    s => s.ToString(),
                    db => (DecisionStatus)Enum.Parse(typeof(DecisionStatus), db))
                .IsRequired();

            // VotingDeadline يمكن تحويله إلى DateTime
            builder.Property(d => d.VotingDeadline)
                   .HasConversion(
                       vd => vd != null ? vd.Value : (DateTime?)null,
                       v => v.HasValue ? VotingDeadline.Of(v.Value) : null
                   )
                   .IsRequired(false);


            // العلاقات مع Votes
            builder.HasMany(d => d.Votes)
                   .WithOne(v => v.Decision)
                   .HasForeignKey(v => v.DecisionId)
                   .IsRequired();
        }
    }
}

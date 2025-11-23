using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingDecisionConfiguration : IEntityTypeConfiguration<MeetingDecision>
    {
        public void Configure(EntityTypeBuilder<MeetingDecision> b)
        {
            b.ToTable("MeetingDecisions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Text).HasMaxLength(4000).IsRequired();

            b.Property(x => x.Outcome)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.GovernanceModel)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.CreatedAt).IsRequired();

            b.HasIndex(x => x.MeetingId);
        }
    }
}

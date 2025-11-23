using MeetingCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingInfrastructure.Configurations
{
    public class MeetingVoteConfiguration : IEntityTypeConfiguration<MeetingVote>
    {
        public void Configure(EntityTypeBuilder<MeetingVote> b)
        {
            b.ToTable("MeetingVotes");
            b.HasKey(x => x.Id);

            b.Property(x => x.MemberId).IsRequired();

            b.Property(x => x.Choice)
                .HasConversion<string>()
                .HasMaxLength(50);

            b.Property(x => x.Timestamp).IsRequired();

            b.HasIndex(x => x.DecisionId);
            b.HasIndex(x => new { x.DecisionId, x.MemberId }).IsUnique(false);
        }
    }
}

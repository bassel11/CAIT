using DecisionCore.Entities;
using DecisionCore.Enums;
using DecisionCore.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DecisionInfrastructure.Data.Configurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            // المفتاح الأساسي
            builder.HasKey(v => v.Id);

            // تحويل VoteId
            builder.Property(v => v.Id)
                .HasConversion(
                    id => id.Value,
                    value => VoteId.Of(value))
                .IsRequired();

            // تحويل DecisionId
            builder.Property(v => v.DecisionId)
                .HasConversion(
                    id => id.Value,
                    value => DecisionId.Of(value))
                .IsRequired();

            // MemberId كـ Guid عادي
            builder.Property(v => v.MemberId)
                .IsRequired();

            // تحويل VoteType إلى string
            builder.Property(v => v.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (VoteType)Enum.Parse(typeof(VoteType), v))
                .IsRequired();

            // VotedAt
            builder.Property(v => v.VotedAt)
                .IsRequired();
        }
    }
}

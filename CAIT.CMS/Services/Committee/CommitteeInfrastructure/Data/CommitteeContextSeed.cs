using CommitteeCore.Entities;
using Microsoft.Extensions.Logging;

namespace CommitteeInfrastructure.Data
{
    public class CommitteeContextSeed
    {
        public static async Task SeedAsync(CommitteeContext context, ILogger<CommitteeContextSeed> logger)
        {
            // لا نقوم بعمل Seed للجنة إذا كانت موجودة
            if (!context.Committees.Any())
            {
                context.Committees.AddRange(GetPreconfiguredCommittees());
                await context.SaveChangesAsync();
                logger.LogInformation($"Committee database seeded with sample data: {typeof(CommitteeContext).Name}");
            }
        }

        private static IEnumerable<Committee> GetPreconfiguredCommittees()
        {
            return new List<Committee>
            {
                new Committee
                {
                    Name = "IT Governance Committee",
                    Purpose = "Oversee IT strategy and alignment with business objectives.",
                    Scope = "Covers IT infrastructure, policies, and digital transformation.",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddYears(1),

                    // New proper Committee Type
                    Type = CommitteeType.Permanent,

                    // New StatusId based on lookup table
                    // 1 = Active  حسب الـ SEED الموجود في DbContext
                    StatusId = 1,

                    Budget = 50000,

                    // Decision data
                    CreationDecisionText = "Approved by the Board."
                }
            };
        }
    }
}

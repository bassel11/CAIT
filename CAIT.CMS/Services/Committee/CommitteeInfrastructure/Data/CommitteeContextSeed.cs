using CommitteeCore.Entities;
using Microsoft.Extensions.Logging;

namespace CommitteeInfrastructure.Data
{
    public class CommitteeContextSeed
    {
        public static async Task SeedAsync(CommitteeContext context, ILogger<CommitteeContextSeed> logger)
        {
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
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    Type = CommitteeType.Permanent,
                    Status = CommitteeStatus.Active,
                    Budget = 50000,
                    CreationDecisionText = "Approved by the Board.",
                    UpdatedDecisionText = null,                  
                }
            };
        }
    }
}

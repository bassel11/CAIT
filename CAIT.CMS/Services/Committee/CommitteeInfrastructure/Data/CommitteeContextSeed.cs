using CommitteeCore.Entities;
using Microsoft.Extensions.Logging;

namespace CommitteeInfrastructure.Data
{
    public class CommitteeContextSeed
    {
        public static async Task SeedAsync(CommitteeContext context, ILogger<CommitteeContextSeed> logger)
        {
            // Seed فقط إذا لم يكن هناك بيانات مسبقة
            if (!context.Committees.Any())
            {
                var committees = GetPreconfiguredCommittees();
                context.Committees.AddRange(committees);
                await context.SaveChangesAsync();
                logger.LogInformation($"Committee database seeded with {committees.Count} sample committees.");
            }
        }

        private static List<Committee> GetPreconfiguredCommittees()
        {
            var random = new Random();
            var committees = new List<Committee>();

            for (int i = 1; i <= 100; i++)
            {
                var startDate = DateTime.UtcNow.AddDays(random.Next(-30, 30));
                var endDate = startDate.AddMonths(random.Next(1, 12));

                committees.Add(new Committee
                {
                    Name = $"Committee {i}",
                    Purpose = $"Purpose for Committee {i}",
                    Scope = $"Scope for Committee {i}",
                    StartDate = startDate,
                    EndDate = endDate,
                    Type = (CommitteeType)random.Next(0, 2), // 0 أو 1
                    StatusId = random.Next(1, 7), // يوجد 6 StatusIds حسب Seed في OnModelCreating
                    Budget = random.Next(1000, 100000),
                    CreationDecisionText = $"Decision text for Committee {i}",
                    UpdatedDecisionText = $"Updated decision text for Committee {i}"
                });
            }

            return committees;
        }
    }
}

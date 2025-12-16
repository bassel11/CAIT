using Microsoft.Extensions.DependencyInjection; // CreateScope, GetRequiredService

namespace DecisionInfrastructure.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            await SeedAsync(context);
        }

        private static async Task SeedAsync(ApplicationDbContext context)
        {
            // هنا ضع بيانات الـ Seed إذا لزم
            await SeedDecision(context);
        }

        private static async Task SeedDecision(ApplicationDbContext context)
        {
            if (!await context.Decisions.AnyAsync())
            {
                await context.Decisions.AddRangeAsync(InitialData.Decisions);
                await context.SaveChangesAsync();
            }
        }
    }
}

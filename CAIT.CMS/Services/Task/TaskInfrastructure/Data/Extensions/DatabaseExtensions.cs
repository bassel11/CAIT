using Microsoft.Extensions.DependencyInjection;

namespace TaskInfrastructure.Data.Extensions
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
            if (!await context.TaskItems.AnyAsync())
            {
                await context.TaskItems.AddRangeAsync(InitialData.Tasks);
                await context.SaveChangesAsync();
            }
        }
    }
}

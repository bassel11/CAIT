using Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Identity.Infrastructure.Data
{
    public static class PermissionSeeder
    {
        public static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "permissions.json");
            Console.WriteLine($"Permission file path: {filePath}");

            if (!File.Exists(filePath))
            {
                Console.WriteLine("permissions.json file not found!");
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var permissions = JsonSerializer.Deserialize<List<Permission>>(json)!;

            foreach (var p in permissions)
            {
                if (!await context.Permissions.AnyAsync(x => x.Name == p.Name))
                    context.Permissions.Add(p);
            }

            await context.SaveChangesAsync();
        }
    }
}

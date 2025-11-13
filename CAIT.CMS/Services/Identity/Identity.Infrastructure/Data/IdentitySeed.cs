using Identity.Core.Entities;
using Identity.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Identity.Core.Entities.ApplicationUser;

namespace Identity.Infrastructure.Data
{
    public class IdentitySeed
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Roles
            string[] roles = new string[] { "SuperAdmin", "CommitteeAdmin", "Member", "Observer", "Rapporteur", "ViceChairman" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} role"
                    });
                }
            }

            // 2. Admin User
            string adminEmail = "admin@cait.gov";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    AuthType = AuthenticationType.Database,
                    MFAEnabled = false,
                    EmailConfirmed = true,
                    IsActive = true,
                    PrivilageType = PrivilageType.PredifinedRoles
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123!"); // strong password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                }
            }

            // 3. ربط جميع الصلاحيات بدور SuperAdmin
            var superAdminRole = await dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == "SuperAdmin");

            if (superAdminRole != null)
            {
                // جميع الصلاحيات الموجودة
                var allPermissions = await dbContext.Permissions.ToListAsync();

                // الصلاحيات الموجودة بالفعل للدور
                var existingPermissionIds = await dbContext.RolePermissions
                    .Where(rp => rp.RoleId == superAdminRole.Id)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();

                // الصلاحيات غير المضافة بعد
                var newPermissions = allPermissions
                    .Where(p => !existingPermissionIds.Contains(p.Id))
                    .Select(p => new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = p.Id
                    })
                    .ToList();

                if (newPermissions.Any())
                {
                    await dbContext.RolePermissions.AddRangeAsync(newPermissions);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}

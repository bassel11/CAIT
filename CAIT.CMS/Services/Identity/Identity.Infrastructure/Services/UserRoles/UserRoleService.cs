using Identity.Application.Interfaces.Authorization;
using Identity.Application.Interfaces.UserRoles;
using Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services.UserRoles
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionCacheInvalidator _cacheInvalidator;
        public UserRoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IPermissionCacheInvalidator cacheInvalidator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _cacheInvalidator = cacheInvalidator;
        }
        public async Task<bool> AssignUserRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _userManager.AddToRoleAsync(user, roleName);

            //if (result.Succeeded)
            //{
            //    // إبطال الكاش بعد إضافة الدور
            //    await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(userId);
            //}

            return result.Succeeded;
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<ApplicationUser>> GetUsersByRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                return new List<ApplicationUser>();

            return (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
        }

        public async Task<bool> RemoveUserRoleAsync(Guid userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);


            //if (result.Succeeded)
            //{
            //    // ✅ إبطال الكاش بعد إزالة الدور
            //    await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(userId);
            //}

            return result.Succeeded;
        }


        #region Multiple 

        public async Task<(bool Success, string Message)> AssignUserRolesAsync(Guid userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return (false, "User not found.");

            // تحقق من أدوار فارغة أو null
            if (roles.Any(r => string.IsNullOrWhiteSpace(r)))
                return (false, "Role names cannot be empty or null.");

            // تحقق من وجود الأدوار في النظام
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            var invalidRoles = roles.Where(r => !allRoles.Contains(r, StringComparer.OrdinalIgnoreCase)).ToList();
            if (invalidRoles.Any())
                return (false, $"The following roles do not exist: {string.Join(", ", invalidRoles)}");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToAdd = roles.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();

            if (!rolesToAdd.Any()) return (true, "All roles are already assigned.");

            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);


            //if (result.Succeeded)
            //{
            //    // إبطال الكاش بعد إضافة أدوار متعددة
            //    await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(userId);
            //}

            return result.Succeeded
                ? (true, "Roles assigned successfully.")
                : (false, "Failed to assign roles.");
        }

        public async Task<bool> RemoveUserRolesAsync(Guid userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = roles.Intersect(currentRoles).ToList(); // فقط الأدوار الموجودة

            if (!rolesToRemove.Any()) return true; // لا يوجد شيء للإزالة

            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            //{
            //    // إبطال الكاش بعد إزالة أدوار متعددة
            //    await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(userId);
            //}

            return result.Succeeded;
        }

        public async Task<bool> AssignUsersToRoleAsync(string roleName, List<Guid> userIds)
        {
            foreach (var id in userIds)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(roleName))
                    {
                        await _userManager.AddToRoleAsync(user, roleName);
                        // await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(id);
                    }
                }
            }
            return true;

        }


        #endregion


    }
}

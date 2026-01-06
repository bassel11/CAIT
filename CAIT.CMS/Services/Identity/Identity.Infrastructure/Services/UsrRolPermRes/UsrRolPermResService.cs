using Identity.Application.Common;
using Identity.Application.DTOs.Pre.Custom;
using Identity.Application.Interfaces.Authorization;
using Identity.Application.Interfaces.Permissions;
using Identity.Application.Interfaces.UsrRolPermRes;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Services.UsrRolPermRes
{
    public class UsrRolPermResService : IUsrRolPermResService
    {
        #region 

        private readonly ApplicationDbContext _context;
        private readonly IPermissionCacheInvalidator _cacheInvalidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IPermissionService _permissionService;


        #endregion

        #region Constructors

        public UsrRolPermResService(ApplicationDbContext context
                                   , IPermissionCacheInvalidator cacheInvalidator
                                   , UserManager<ApplicationUser> userManager
                                   , RoleManager<ApplicationRole> roleManager
                                   , IPermissionService permissionService)
        {
            _context = context;
            _cacheInvalidator = cacheInvalidator;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionService = permissionService;
        }

        #endregion

        #region Actions

        public async Task<bool> AssignCustomPermsAsync(AssignCustomPermsDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null || user.PrivilageType != PrivilageType.CustomRolesAndPermission)
                return false;
            //throw new ValidationException("User not found or not eligible for custom privileges.");

            // يجب وضع كود يتحقق من ان ليس له بيانات في جدول UserRole !!!!!!!!
            var existUserRoles = await _userManager.GetRolesAsync(user);
            if (existUserRoles != null && existUserRoles.Any())
            {
                return false;
            }
            // // // //

            var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
            if (role == null)
                return false;

            foreach (var item in dto.Permissions)
            {
                if (!await _permissionService.ExistsAsync(item.PermissionId))
                    throw new KeyNotFoundException($"Permission {item.PermissionId} not found");

                // التحقق من النوع Scope
                await ValidateScopeAsync(item);

                bool alreadyExists = await _context.UserRolePermResos.AnyAsync(rp =>
                    rp.UserId == dto.UserId &&
                    rp.RoleId == dto.RoleId &&
                    rp.PermissionId == item.PermissionId &&
                    rp.Scope == item.Scope &&
                    rp.ResourceId == item.ResourceId &&
                    rp.ParentResourceId == item.ParentResourceId);

                if (alreadyExists)
                    continue;

                var newUsrRolePermReso = new UserRolePermReso
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId,
                    PermissionId = item.PermissionId,
                    Scope = item.Scope,
                    ResourceId = item.ResourceId,
                    ParentResourceId = item.ParentResourceId,
                    Allow = item.Allow
                };

                _context.UserRolePermResos.Add(newUsrRolePermReso);

            }

            await _context.SaveChangesAsync();

            // مسح كاش الصلاحيات للمستخدمين الذين ينتمون لهذا الدور
            // تم ايقافه لان inteceptor هو من يقوم بالنشر
            //await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(dto.UserId);

            return true;

        }

        public async Task<IEnumerable<CustomPermsDetailsDto>> GetCustomPermsAsync(Guid UserId, CustomPermFilterDto? filter = null)
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null || user.PrivilageType != PrivilageType.CustomRolesAndPermission)
                throw new KeyNotFoundException($"User {UserId} not found or belongs to PredifinedRoles");

            var query = _context.UserRolePermResos
                                .Include(x => x.Role)
                                .Include(x => x.Permission)
                                .Where(x => x.UserId == UserId)
                                .AsQueryable();
            // Filters
            if (filter != null)
            {
                // البحث النصي
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var s = filter.Search.Trim().ToLower();
                    query = query.Where(x =>
                        x.Permission.Name.ToLower().Contains(s) ||
                        x.Role.Name!.ToLower().Contains(s));
                }

                // RoleId
                if (filter.RoleId != Guid.Empty)
                    query = query.Where(x => x.RoleId == filter.RoleId);

                // PermissionId
                if (filter.PermissionId != Guid.Empty)
                    query = query.Where(x => x.PermissionId == filter.PermissionId);

                // Scope
                if (filter.Scope != ScopeType.Global)
                    query = query.Where(x => x.Scope == filter.Scope);

                // السماح Allow
                query = query.Where(x => x.Allow == filter.Allow);

                //  الترتيب 
                var sortMap = new Dictionary<string, Expression<Func<UserRolePermReso, object>>>
                {
                    ["username"] = x => x.User.UserName!,
                    ["rolename"] = x => x.Role.Name!,
                    ["permissionname"] = x => x.Permission.Name,
                    ["scope"] = x => x.Scope,
                    ["allow"] = x => x.Allow
                };

                query = query.ApplySorting(filter.SortBy!, filter.SortDir, sortMap);

                // التصفح Pagination
                query = query.ApplyPaging(filter);
            }
            // التحويل باستخدام Mapper
            var result = await query.Select(CustomPermsMapper.ToDtoExpr).ToListAsync();
            return result;

        }

        public async Task<bool> RemoveCustomPermsAsync(RemoveCustomPermsDto dto)
        {
            //  تحقق أولاً من المستخدم وصلاحية النوع
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null || user.PrivilageType != PrivilageType.CustomRolesAndPermission)
                throw new KeyNotFoundException($"User {dto.UserId} not found or does not have CustomRolesAndPermission.");

            //  البحث عن السجل المحدد بدقة
            var entity = await _context.UserRolePermResos
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.UserRolePermResoId &&
                    x.UserId == dto.UserId &&
                    x.RoleId == dto.RoleId &&
                    x.PermissionId == dto.PermissionId);

            if (entity == null)
                throw new KeyNotFoundException("Key not found or already deleted.");

            //  إزالة السجل
            _context.UserRolePermResos.Remove(entity);

            await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(dto.UserId);

            await _context.SaveChangesAsync();

            // مسح كاش الصلاحيات للمستخدمين الذين ينتمون لهذا الدور
            //await _cacheInvalidator.InvalidateUserPermissionsByUserAsync(dto.UserId);
            return true;
        }

        public async Task<bool> HasCustomPermissionsAsync(Guid userId)
        {
            return await _context.UserRolePermResos.AnyAsync(x => x.UserId == userId);
        }

        #endregion

        #region Private Functions

        private async Task ValidateScopeAsync(CustomPermsDto item)
        {
            switch (item.Scope)
            {
                case ScopeType.Global:
                    item.ResourceId = null;
                    item.ParentResourceId = null;
                    break;

                case ScopeType.ResourceOnly:
                    if (item.ResourceId == null)
                        throw new ValidationException("ResourceId is required when Scope = ResourceOnly.");
                    item.ParentResourceId = null;
                    break;

                case ScopeType.ResourceWithParent:
                    if (item.ResourceId == null || item.ParentResourceId == null)
                        throw new ValidationException("ResourceId and ParentResourceId is required when Scope = ResourceWithParent.");
                    break;

                default:
                    throw new ValidationException("Invalid ScopeType value.");
            }
        }

        #endregion
    }
}

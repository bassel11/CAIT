using Identity.Application.Common;
using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.Authorization;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Services.RolePermissions
{
    public class RolePremissionService : IRolePremissionService
    {
        #region

        private readonly ApplicationDbContext _context;
        private readonly IPermissionCacheInvalidator _cacheInvalidator;

        #endregion

        #region Constructors
        public RolePremissionService(ApplicationDbContext context, IPermissionCacheInvalidator cacheInvalidator)
        {
            _context = context;
            _cacheInvalidator = cacheInvalidator;

        }
        #endregion

        #region Actions

        public async Task<bool> AsgnPermsToRoleAsync(AsgnPermsToRoleDto dto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null)
                throw new KeyNotFoundException("Role not found");

            // إضافة الصلاحيات الجديدة
            foreach (var item in dto.Permissions)
            {
                var permissionExists = await _context.Permissions.AnyAsync(p => p.Id == item.PermissionId);
                if (!permissionExists)
                    throw new KeyNotFoundException($"Permission {item.PermissionId} not found");

                //  التحقق من القيم بناءً على ScopeType
                //await ValidateScopeAsync(item);

                //  منع التكرار لنفس الدور + الصلاحية + النطاق
                bool alreadyExists = await _context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == dto.RoleId &&
                    rp.PermissionId == item.PermissionId);

                if (alreadyExists)
                    continue;

                var newRolePermission = new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = item.PermissionId
                };

                _context.RolePermissions.Add(newRolePermission);
            }

            await _context.SaveChangesAsync();

            // مسح كاش الصلاحيات للمستخدمين الذين ينتمون لهذا الدور
            await _cacheInvalidator.InvalidateUserPermissionsByRoleAsync(dto.RoleId);

            return true;
        }
        public async Task<IEnumerable<PermsDetailsDto>> GetPermsByRoleAsync(PermsByRoleFilterDto? filter = null)
        {
            if (filter?.RoleId == null)
                throw new ValidationException("RoleId is required.");

            // تحقق من وجود الدور
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == filter.RoleId);
            if (!roleExists)
                throw new KeyNotFoundException($"Role {filter.RoleId} not found");

            // ابدأ الاستعلام مع التحميل الضروري
            var query = _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == filter.RoleId)
                .AsQueryable();

            // =========================
            //  الفلاتر Filters
            // =========================
            if (filter != null)
            {
                //  البحث النصي
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var s = filter.Search.Trim();
                    query = query.Where(rp =>
                        rp.Permission.Name.Contains(s) ||
                        rp.Permission.Description.Contains(s));
                }

                //  نوع المورد (ResourceType)
                if (filter.ResourceType.HasValue)
                    query = query.Where(rp => rp.Permission.ResourceType == filter.ResourceType.Value);

                //  نوع العملية (Action)
                if (filter.Action.HasValue)
                    query = query.Where(rp => rp.Permission.Action == filter.Action.Value);

                //  حالة التفعيل
                if (filter.IsActive.HasValue)
                    query = query.Where(rp => rp.Permission.IsActive == filter.IsActive.Value);

                //  صلاحيات عامة أو خاصة
                if (filter.IsGlobal.HasValue)
                    query = query.Where(rp => rp.Permission.IsGlobal == filter.IsGlobal.Value);

                // الترتيب Sorting
                var sortMap = new Dictionary<string, Expression<Func<RolePermission, object>>>
                {
                    ["name"] = rp => rp.Permission.Name,
                    ["description"] = rp => rp.Permission.Description,
                    ["resourcetype"] = rp => rp.Permission.ResourceType,
                    ["action"] = rp => rp.Permission.Action,
                    ["isglobal"] = rp => rp.Permission.IsGlobal,
                    ["isactive"] = rp => rp.Permission.IsActive
                };

                query = query.ApplySorting(filter.SortBy!, filter.SortDir, sortMap);

                // =========================
                //  التصفح Pagination
                // =========================
                query = query.ApplyPaging(filter);
            }


            // التحويل باستخدام Mapper
            var result = await query.Select(RolePermissionMapper.ToDtoExpr).ToListAsync();
            return result;
        }

        public async Task<bool> RemovePermissionsOfRoleAsync(RemPermsToRoleDto dto)
        {
            // التحقق من وجود الصلاحية المرتبطة بالدور
            var preRolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp =>
                    rp.RoleId == dto.RoleId &&
                    rp.PermissionId == dto.PermissionId);

            if (preRolePermission == null)
                throw new KeyNotFoundException("Role permission not found.");

            // حفظ التغييرات
            await _context.SaveChangesAsync();

            await _cacheInvalidator.InvalidateUserPermissionsByRoleAsync(dto.RoleId);

            return true;
        }

        #endregion
    }
}

using Identity.Application.Common;
using Identity.Application.DTOs.Permissions;
using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Services.RolePermissions
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly ApplicationDbContext _context;
        public RolePermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignPermissionsToRoleAsync(AssignPermissionsToRoleDto dto)
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
                await ValidateScopeAsync(item);

                //  منع التكرار لنفس الدور + الصلاحية + النطاق
                bool alreadyExists = await _context.RolePermissions.AnyAsync(rp =>
                    rp.RoleId == dto.RoleId &&
                    rp.PermissionId == item.PermissionId &&
                    rp.ScopeType == item.ScopeType &&
                    rp.ResourceId == item.ResourceId);

                if (alreadyExists)
                    continue;

                var newRolePermission = new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = item.PermissionId,
                    ScopeType = item.ScopeType,
                    //CommitteeId = item.CommitteeId,
                    ResourceId = item.ResourceId,
                    Allow = item.Allow
                };

                _context.RolePermissions.Add(newRolePermission);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(PermissionByRoleFilterDto? filter = null)
        {
            var query = _context.RolePermissions
                .Where(rp => rp.RoleId == filter.RoleId)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .AsQueryable();

            if (filter != null)
            {
                // 🔹 البحث النصي
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var s = filter.Search.Trim();
                    query = query.Where(p => p.Name.Contains(s) || p.Description.Contains(s));
                }

                // 🔹 الفلاتر الأخرى
                if (filter.ResourceType.HasValue)
                    query = query.Where(p => p.ResourceType == filter.ResourceType.Value);

                if (filter.Action.HasValue)
                    query = query.Where(p => p.Action == filter.Action.Value);

                if (filter.IsActive.HasValue)
                    query = query.Where(p => p.IsActive == filter.IsActive.Value);

                if (filter.IsGlobal.HasValue)
                    query = query.Where(p => p.IsGlobal == filter.IsGlobal.Value);

                // 🔹 ترتيب النتائج
                var sortMap = new Dictionary<string, Expression<Func<Permission, object>>>
                {
                    ["name"] = p => p.Name,
                    ["createdat"] = p => p.CreatedAt,
                    ["resource"] = p => p.ResourceType,
                    ["action"] = p => p.Action,
                    ["isglobal"] = p => p.IsGlobal,
                    ["isactive"] = p => p.IsActive
                };

                query = query.ApplySorting(filter.SortBy!, filter.SortDir, sortMap);

                // 🔹 تطبيق Paging
                query = query.ApplyPaging(filter);
            }

            // Projection باستخدام Mapper
            var permissions = await query.Select(PermissionMapper.ToDtoExpr).ToListAsync();

            return permissions;
        }

        public async Task<IEnumerable<RolePermissionDetailsDto>> GetRolePermissionsWithResourcesAsync(PermissionByRoleFilterDto? filter = null)
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
                .Include(rp => rp.Resource)
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
                        rp.Permission.Description.Contains(s) ||
                        (rp.Resource != null && rp.Resource.DisplayName!.Contains(s)));
                }

                //  نوع المورد (ResourceType)
                if (filter.ResourceType.HasValue)
                    query = query.Where(rp => rp.Permission.ResourceType == filter.ResourceType.Value);

                //  نوع العملية (Action)
                if (filter.Action.HasValue)
                    query = query.Where(rp => rp.Permission.Action == filter.Action.Value);

                //  نوع النطاق (ScopeType)
                if (filter.ScopeType.HasValue)
                    query = query.Where(rp => rp.ScopeType == filter.ScopeType.Value);

                //  حالة التفعيل
                if (filter.IsActive.HasValue)
                    query = query.Where(rp => rp.Permission.IsActive == filter.IsActive.Value);

                //  صلاحيات عامة أو خاصة
                if (filter.IsGlobal.HasValue)
                    query = query.Where(rp => rp.Permission.IsGlobal == filter.IsGlobal.Value);

                // المورد (ResourceId)
                if (filter.ResourceId.HasValue)
                    query = query.Where(rp => rp.ResourceId == filter.ResourceId.Value);

                // السماح (Allow)
                if (filter.Allow.HasValue)
                    query = query.Where(rp => rp.Allow == filter.Allow.Value);

                // الترتيب Sorting
                var sortMap = new Dictionary<string, Expression<Func<RolePermission, object>>>
                {
                    ["name"] = rp => rp.Permission.Name,
                    ["description"] = rp => rp.Permission.Description,
                    ["resource"] = rp => rp.Resource.DisplayName!,
                    ["resourcetype"] = rp => rp.Permission.ResourceType,
                    ["action"] = rp => rp.Permission.Action,
                    ["scope"] = rp => rp.ScopeType,
                    ["isglobal"] = rp => rp.Permission.IsGlobal,
                    ["isactive"] = rp => rp.Permission.IsActive,
                    ["allow"] = rp => rp.Allow,
                    ["createdat"] = rp => rp.CreatedAt
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

        public async Task<bool> UpdateRolePermissionResourceAsync(UpdateRolePermissionResourceDto dto)
        {
            // التحقق من وجود الصلاحية المرتبطة بالدور
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp =>
                    rp.RoleId == dto.RoleId &&
                    rp.PermissionId == dto.PermissionId);

            if (rolePermission == null)
                throw new KeyNotFoundException("Role permission not found.");

            // التحقق من وجود المورد
            var resource = await _context.Resources.FirstOrDefaultAsync(r => r.Id == dto.ResourceId);
            if (resource == null)
                throw new ValidationException($"Resource {dto.ResourceId} not found.");

            // تعديل القيم
            rolePermission.ScopeType = PermissionScopeType.ResourceInstance;
            rolePermission.ResourceId = dto.ResourceId;
            if (dto.Allow.HasValue)
                rolePermission.Allow = dto.Allow.Value;

            // حفظ التغييرات
            await _context.SaveChangesAsync();
            return true;
        }


        //  دالة التحقق من صلاحية النطاق
        private async Task ValidateScopeAsync(RolePermissionItemDto item)
        {
            switch (item.ScopeType)
            {
                case PermissionScopeType.Global:
                    // لا يجب أن يكون هناك CommitteeId أو ResourceId
                    //item.CommitteeId = null;
                    item.ResourceId = null;
                    break;

                case PermissionScopeType.ResourceType:
                    // يجب أن تحتوي على CommitteeId فقط

                    item.ResourceId = null;
                    break;

                case PermissionScopeType.ResourceInstance:
                    // يجب أن تحتوي على ResourceId
                    if (item.ResourceId == null)
                        throw new ValidationException("ResourceId is required when ScopeType = ResourceInstance.");

                    // التأكد من أن المورد موجود
                    var resource = await _context.Resources
                        .FirstOrDefaultAsync(r => r.Id == item.ResourceId);

                    if (resource == null)
                        throw new ValidationException($"Resource {item.ResourceId} not found.");
                    break;

                default:
                    throw new ValidationException("Invalid ScopeType value.");
            }
        }

    }
}

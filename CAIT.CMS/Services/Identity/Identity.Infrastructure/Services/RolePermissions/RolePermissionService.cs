using Identity.Application.Common;
using Identity.Application.DTOs.Permissions;
using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

            if (role == null) throw new KeyNotFoundException("Role not found");

            var existingPermissions = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            var newPermissions = dto.PermissionIds.Except(existingPermissions).ToList();

            foreach (var permId in newPermissions)
            {
                if (!await _context.Permissions.AnyAsync(p => p.Id == permId))
                    throw new KeyNotFoundException($"Permission {permId} not found");

                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId, PermissionByRoleFilterDto? filter = null)
        {
            var query = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
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
                if (filter.Resource.HasValue)
                    query = query.Where(p => p.Resource == filter.Resource.Value);

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
                    ["resource"] = p => p.Resource,
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

        //private static PermissionDto MapToDto(Permission p)
        //{
        //    return new PermissionDto
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Description = p.Description,
        //        Resource = p.Resource.ToString(),
        //        Action = p.Action.ToString(),
        //        IsGlobal = p.IsGlobal,
        //        IsActive = p.IsActive
        //    };
        //}
    }
}

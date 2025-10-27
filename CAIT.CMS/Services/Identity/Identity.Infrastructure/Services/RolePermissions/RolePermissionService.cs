using Identity.Application.DTOs.Permissions;
using Identity.Application.DTOs.RolePermissions;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(Guid roleId)
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                //.Select(rp => MapToDto(rp.Permission))
                .Select(rp => PermissionMapper.ToDto(rp.Permission))
                .ToListAsync();

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

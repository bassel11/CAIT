using Identity.Application.Common;
using Identity.Application.DTOs.Roles;
using Identity.Application.Interfaces.Roles;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, ILogger<RoleService> logger)
        {
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task<PagedResult<RoleDto>> GetRolesAsync(RoleFilterDto filter)
        {
            var query = _context.Roles
                .Include(r => r.UserRoles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(r => r.Name.Contains(filter.Search) || (r.Description ?? "").Contains(filter.Search));

            var total = await query.CountAsync();

            var roles = await query
                .OrderBy(r => r.Name)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name!,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UserCount = r.UserRoles.Count
                })
                .ToListAsync();

            return new PagedResult<RoleDto>(roles, total, filter.Page, filter.PageSize);
        }

        public async Task<RoleDto?> GetByIdAsync(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            return role == null ? null : new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UserCount = role.UserRoles.Count
            };
        }

        public async Task<(bool Success, string? Error)> CreateAsync(RoleCreateDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.Name))
                return (false, "Role already exists");

            var role = new ApplicationRole
            {
                Name = dto.Name,
                NormalizedName = dto.Name.ToUpperInvariant(),
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create role: {Error}", error);
                return (false, error);
            }

            _logger.LogInformation("Role {RoleName} created successfully", dto.Name);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Guid id, RoleUpdateDto dto)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                return (false, "Role not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (await _roleManager.RoleExistsAsync(dto.Name) && role.Name != dto.Name)
                    return (false, "Another role with this name already exists");

                role.Name = dto.Name;
                role.NormalizedName = dto.Name.ToUpperInvariant();
            }

            role.Description = dto.Description ?? role.Description;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to update role {RoleId}: {Error}", id, error);
                return (false, error);
            }

            _logger.LogInformation("Role {RoleId} updated successfully", id);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                return (false, "Role not found");

            if (await _context.UserRoles.AnyAsync(ur => ur.RoleId == id))
                return (false, "Cannot delete role because it is assigned to one or more users.");

            // Force Prevent Delete
            // return (false, "Now You Cannot Delete Roles Please, Please check with the developer or system administrator. .");
            //

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to delete role {RoleId}: {Error}", id, error);
                return (false, error);
            }

            _logger.LogInformation("Role {RoleId} deleted successfully", id);
            return (true, null);
        }
    }
}

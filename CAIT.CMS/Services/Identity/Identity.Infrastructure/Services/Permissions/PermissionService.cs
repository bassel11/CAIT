using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces.Permissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
        {
            if (await _context.Permissions.AnyAsync(p => p.Name == dto.Name))
                throw new ApplicationException("Permission with the same name already exists.");

            var permission = new Permission
            {
                Name = dto.Name,
                Description = dto.Description,
                Resource = dto.Resource,
                Action = dto.Action,
                IsGlobal = dto.IsGlobal,
                IsActive = true
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            //return MapToDto(permission);
            return PermissionMapper.ToDto(permission);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) return false;

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                //.Select(p => MapToDto(p))
                .Select(p => PermissionMapper.ToDto(p))
                .ToListAsync();
        }

        public async Task<PermissionDto> GetByIdAsync(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) throw new KeyNotFoundException("Permission not found");
            //return MapToDto(permission);
            return PermissionMapper.ToDto(permission);
        }

        public async Task<PermissionDto> UpdateAsync(Guid id, UpdatePermissionDto dto)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null) throw new KeyNotFoundException("Permission not found");

            permission.Name = dto.Name;
            permission.Description = dto.Description;
            permission.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            //return MapToDto(permission);
            return PermissionMapper.ToDto(permission);
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

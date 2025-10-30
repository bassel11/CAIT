using Identity.Application.Common;
using Identity.Application.DTOs.Permissions;
using Identity.Application.Interfaces.Permissions;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Identity.Infrastructure.Services.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        //private const int MaxPageSize = 200;

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
                ResourceType = dto.ResourceType,
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
            if (permission == null)
                return false;

            // بدلاً من الحذف الفيزيائي، نعطل الصلاحية
            if (!permission.IsActive)
                return true; // أو false حسب ما تريد (إذا كانت معطلة مسبقاً)

            permission.IsActive = false;
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


        public async Task<PagedResult<PermissionDto>> GetPagedAsync(PermissionFilterDto filter)
        {
            var query = _context.Permissions.AsNoTracking();

            // filters
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                query = query.Where(p => p.Name.Contains(s) || p.Description.Contains(s));
            }
            if (filter.Resource.HasValue)
                query = query.Where(p => p.ResourceType == filter.Resource.Value);
            if (filter.Action.HasValue)
                query = query.Where(p => p.Action == filter.Action.Value);
            if (filter.IsActive.HasValue)
                query = query.Where(p => p.IsActive == filter.IsActive.Value);

            var total = await query.CountAsync();

            // sorting
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
            query = query.ApplyPaging(filter);

            var permissions = await query.Select(PermissionMapper.ToDtoExpr).ToListAsync();

            return new PagedResult<PermissionDto>(permissions, total, filter.Page, filter.PageSize);
        }
    }
}

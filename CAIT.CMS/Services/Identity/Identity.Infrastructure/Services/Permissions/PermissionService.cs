using Identity.Application.Common;
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
            // sanitize pageSize
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1
                ? PaginationDefaults.DefaultPageSize
                : Math.Min(filter.PageSize, PaginationDefaults.MaxPageSize);

            IQueryable<Permission> query = _context.Permissions.AsNoTracking();

            // Filters
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var s = filter.Search.Trim();
                // use EF functions if desired (e.g., EF.Functions.Like) for performance
                query = query.Where(p => p.Name.Contains(s) || p.Description.Contains(s));
            }

            if (filter.Resource.HasValue)
                query = query.Where(p => p.Resource == filter.Resource.Value);

            if (filter.Action.HasValue)
                query = query.Where(p => p.Action == filter.Action.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(p => p.IsActive == filter.IsActive.Value);

            if (filter.IsGlobal.HasValue)
                query = query.Where(p => p.IsGlobal == filter.IsGlobal.Value);

            // Count before paging
            var totalCount = await query.CountAsync();

            // Sorting
            // normalize
            var sortBy = (filter.SortBy ?? "name").ToLowerInvariant();
            var sortDir = (filter.SortDir ?? "asc").ToLowerInvariant();

            // Default sort if unknown
            switch (sortBy)
            {
                case "createdat":
                case "created":
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                    break;
                case "resource":
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Resource) : query.OrderBy(p => p.Resource);
                    break;
                case "action":
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Action) : query.OrderBy(p => p.Action);
                    break;
                case "isglobal":
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.IsGlobal) : query.OrderBy(p => p.IsGlobal);
                    break;
                case "isactive":
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.IsActive) : query.OrderBy(p => p.IsActive);
                    break;
                case "name":
                default:
                    query = sortDir == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                    break;
            }

            // Paging
            var skip = (page - 1) * pageSize;

            // Projection using Expression mapper to translate to SQL
            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(PermissionMapper.ToDtoExpr)
                .ToListAsync();

            return new PagedResult<PermissionDto>(items, totalCount, page, pageSize);
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

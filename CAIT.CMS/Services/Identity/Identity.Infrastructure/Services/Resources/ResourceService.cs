using Identity.Application.DTOs.Resources;
using Identity.Application.Interfaces.Resources;
using Identity.Application.Mappers;
using Identity.Core.Entities;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Services.Resources
{
    public class ResourceService : IResourceService
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructors
        public ResourceService(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Actions
        public async Task<ResourceGetDto> CreateResourceAsync(ResourceCreateDto dto, Guid userId)
        {
            var resource = new Resource
            {
                ResourceType = dto.ResourceType,
                ReferenceId = dto.ReferenceId,
                ParentResourceType = dto.ParentResourceType,
                ParentReferenceId = dto.ParentReferenceId,
                DisplayName = dto.DisplayName,
                MetadataJson = dto.MetadataJson,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            return ResourceMapper.ToDto(resource);
        }

        public async Task<ResourceGetDto> UpdateResourceAsync(Guid id, ResourceUpdateDto dto)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                throw new KeyNotFoundException("Resource not found");

            resource.ResourceType = dto.ResourceType;
            resource.ReferenceId = dto.ReferenceId;
            resource.ParentResourceType = dto.ParentResourceType;
            resource.ParentReferenceId = dto.ParentReferenceId;
            resource.DisplayName = dto.DisplayName;
            resource.MetadataJson = dto.MetadataJson;
            resource.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ResourceMapper.ToDto(resource);
        }

        public async Task<ResourceGetDto?> GetResourceByIdAsync(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            return resource == null ? null : ResourceMapper.ToDto(resource);
        }

        public async Task<IEnumerable<ResourceGetDto>> GetAllResourcesAsync()
        {
            return await _context.Resources
                .Select(ResourceMapper.ToDtoExpr)
                .ToListAsync();
        }

        public async Task DeleteResourceAsync(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                throw new KeyNotFoundException("Resource not found");

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

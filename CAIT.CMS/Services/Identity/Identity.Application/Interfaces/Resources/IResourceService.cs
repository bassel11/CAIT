using Identity.Application.DTOs.Resources;

namespace Identity.Application.Interfaces.Resources
{
    public interface IResourceService
    {
        Task<ResourceGetDto> CreateResourceAsync(ResourceCreateDto dto, Guid userId);
        Task<ResourceGetDto> UpdateResourceAsync(Guid id, ResourceUpdateDto dto);
        Task<ResourceGetDto?> GetResourceByIdAsync(Guid id);
        Task<IEnumerable<ResourceGetDto>> GetAllResourcesAsync();
        Task DeleteResourceAsync(Guid id);
    }
}

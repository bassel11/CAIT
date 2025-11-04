using Identity.Application.DTOs.Resources;
using Identity.Core.Entities;
using System.Linq.Expressions;

namespace Identity.Application.Mappers
{
    public static class ResourceMapper
    {
        // Expression usable in IQueryable for efficient projection
        public static readonly Expression<Func<Resource, ResourceGetDto>> ToDtoExpr = r => new ResourceGetDto
        {
            Id = r.Id,
            ResourceType = r.ResourceType,
            ReferenceId = r.ReferenceId,
            ParentResourceType = r.ParentResourceType,
            ParentReferenceId = r.ParentReferenceId,
            DisplayName = r.DisplayName,
            MetadataJson = r.MetadataJson,
            CreatedAt = r.CreatedAt,
            CreatedBy = r.CreatedBy,
            UpdatedAt = r.UpdatedAt
        };

        // Helper if you have a fully-loaded entity
        public static ResourceGetDto ToDto(Resource? r)
        {
            if (r == null) return null!;
            return ToDtoExpr.Compile().Invoke(r);
        }
    }
}

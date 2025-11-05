using Identity.Core.Enums;

namespace Identity.Application.DTOs.Resources
{
    public class ResourceGetDto
    {
        public Guid Id { get; set; }
        public ResourceType ResourceType { get; set; }
        public Guid ReferenceId { get; set; }
        public ResourceType? ParentResourceType { get; set; }
        public Guid? ParentReferenceId { get; set; }
        public string? DisplayName { get; set; }
        public string? MetadataJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

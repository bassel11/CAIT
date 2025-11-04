using Identity.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs.Resources
{
    public class ResourceUpdateDto
    {
        [Required]
        public ResourceType ResourceType { get; set; }

        [Required]
        public Guid ReferenceId { get; set; }

        public ResourceType ParentResourceType { get; set; }
        public Guid? ParentReferenceId { get; set; }

        [MaxLength(300)]
        public string? DisplayName { get; set; }

        public string? MetadataJson { get; set; }
    }
}

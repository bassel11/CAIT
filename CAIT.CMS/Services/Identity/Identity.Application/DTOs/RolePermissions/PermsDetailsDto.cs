using Identity.Core.Enums;

namespace Identity.Application.DTOs.RolePermissions
{
    public class PermsDetailsDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ResourceType PermissionResourceType { get; set; }
        public string PermissionResourceTypeName { get; set; } = string.Empty;

        public ActionType Action { get; set; }
        public string ActionName { get; set; } = string.Empty;

        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }

    }
}

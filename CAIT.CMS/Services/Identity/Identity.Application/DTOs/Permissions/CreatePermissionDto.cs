using Identity.Core.Enums;

namespace Identity.Application.DTOs.Permissions
{
    public class CreatePermissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ResourceType Resource { get; set; }
        public ActionType Action { get; set; }
        public bool IsGlobal { get; set; } = false;
    }
}

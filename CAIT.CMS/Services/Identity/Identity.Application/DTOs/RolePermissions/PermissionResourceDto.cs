namespace Identity.Application.DTOs.RolePermissions
{
    public class PermissionResourceDto
    {
        public string PermissionName { get; set; } = string.Empty;
        public Guid ResourceId { get; set; }
    }
}

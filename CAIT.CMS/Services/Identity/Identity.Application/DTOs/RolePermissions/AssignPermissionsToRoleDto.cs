namespace Identity.Application.DTOs.RolePermissions
{
    public class AssignPermissionsToRoleDto
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new List<Guid>();
    }
}

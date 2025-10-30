namespace Identity.Application.DTOs.RolePermissions
{
    public class AssignPermissionsToRoleDto
    {
        public Guid RoleId { get; set; }
        // قائمة من الصلاحيات مع تفاصيل النطاق ونوع التفويض
        public List<RolePermissionItemDto> Permissions { get; set; } = new();
    }
}

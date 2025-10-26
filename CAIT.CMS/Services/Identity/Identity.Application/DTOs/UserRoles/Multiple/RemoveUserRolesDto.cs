namespace Identity.Application.DTOs.UserRoles.Multiple
{
    public class RemoveUserRolesDto
    {
        public Guid UserId { get; set; }
        public List<string> RoleNames { get; set; } // قائمة الأدوار للحذف
    }
}

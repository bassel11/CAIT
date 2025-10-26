namespace Identity.Application.DTOs.UserRoles.Multiple
{
    public class AssignUserRolesDto
    {
        public Guid UserId { get; set; }
        public List<string> RoleNames { get; set; } // قائمة الأدوار
    }
}

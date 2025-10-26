namespace Identity.Application.DTOs.UserRoles.Multiple
{
    public class AssignUsersToRoleDto
    {
        public string RoleName { get; set; }
        public List<Guid> UserIds { get; set; } // قائمة المستخدمين
    }
}

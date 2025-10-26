namespace Identity.Application.DTOs.UserRoles
{
    public class RemoveUserRoleDto
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; }
    }
}

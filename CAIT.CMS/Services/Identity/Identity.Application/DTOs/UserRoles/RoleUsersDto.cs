namespace Identity.Application.DTOs.UserRoles
{
    public class RoleUsersDto
    {
        public string RoleName { get; set; }
        public IEnumerable<UserRolesDto> Users { get; set; } = new List<UserRolesDto>();
    }
}

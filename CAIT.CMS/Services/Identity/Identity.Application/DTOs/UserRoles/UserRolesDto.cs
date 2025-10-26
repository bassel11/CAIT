namespace Identity.Application.DTOs.UserRoles
{
    public class UserRolesDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}

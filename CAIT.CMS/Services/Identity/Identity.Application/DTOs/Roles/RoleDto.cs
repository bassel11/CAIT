namespace Identity.Application.DTOs.Roles
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserCount { get; set; }
    }
}

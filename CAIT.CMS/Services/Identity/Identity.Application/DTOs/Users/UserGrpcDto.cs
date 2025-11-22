namespace Identity.Application.DTOs.Users
{
    public class UserGrpcDto
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}

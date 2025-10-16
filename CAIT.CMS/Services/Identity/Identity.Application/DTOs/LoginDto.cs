namespace Identity.Application.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int AuthType { get; set; } = 0; // 0 = Database, 1 = LDAP ...
    }
}

using Identity.Core.Enums;

namespace Identity.Application.DTOs
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public PrivilageType privilageType { get; set; } = PrivilageType.None;
        public string Role { get; set; } = string.Empty; // Role
    }
}

using Identity.Core.Enums;

namespace Identity.Application.DTOs.Users
{
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public bool? MFAEnabled { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public PrivilageType PrivilageType { get; set; }
    }
}

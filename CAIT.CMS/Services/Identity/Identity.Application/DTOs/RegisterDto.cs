using Identity.Core.Enums; // تأكد من وجود الـ Enum هنا
using System.ComponentModel.DataAnnotations;

namespace Identity.Application.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // إضافة نوع المستخدم
        [Required]
        public UserType UserType { get; set; }

        public PrivilageType privilageType { get; set; } = PrivilageType.None;

        public string Role { get; set; } = string.Empty;
    }
}
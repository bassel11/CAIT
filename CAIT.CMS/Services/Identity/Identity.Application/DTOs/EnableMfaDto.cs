using Identity.Core.Enums;

namespace Identity.Application.DTOs
{
    public class EnableMfaDto
    {
        public string UserId { get; set; } = string.Empty; // Id المستخدم
        public MFAMethod DeliveryMethod { get; set; } = MFAMethod.Email; // or MFAMethod.TOTP
    }
}

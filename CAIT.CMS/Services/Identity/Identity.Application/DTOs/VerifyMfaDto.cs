namespace Identity.Application.DTOs
{
    public class VerifyMfaDto
    {
        public string UserId { get; set; } = string.Empty; // Id المستخدم
        public string Code { get; set; } = string.Empty;   // الكود المرسل
    }
}

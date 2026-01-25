namespace Identity.Application.DTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResendMfaCodeDto
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class ForceResetPasswordDto
    {
        public string UserId { get; set; } // أو Email
        public string NewPassword { get; set; }
    }
}

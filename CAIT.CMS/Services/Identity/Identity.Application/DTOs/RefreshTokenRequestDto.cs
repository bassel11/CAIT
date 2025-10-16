namespace Identity.Application.DTOs
{
    public class RefreshTokenRequestDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int AuthType { get; set; } = 0; // 0=Database, 1=LDAP
    }
}

namespace Identity.Application.DTOs
{
    public class LdapLoginResponseDto
    {
        public bool Success { get; set; }
        public string? ExternalId { get; set; }
        public string? Error { get; set; }
    }
}

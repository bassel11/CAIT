namespace Identity.Application.DTOs
{
    public class EnableMfaDto
    {
        public string UserId { get; set; } = string.Empty; // Id المستخدم
        public string DeliveryMethod { get; set; } = "Email"; // or "App"
    }
}

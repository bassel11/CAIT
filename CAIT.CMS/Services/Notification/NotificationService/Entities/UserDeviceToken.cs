namespace NotificationService.Entities
{
    public class UserDeviceToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceToken { get; set; } = string.Empty; // رمز Firebase
        public string Platform { get; set; } = string.Empty; // "Android", "iOS", "Web"
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}

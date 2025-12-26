namespace NotificationService.Services
{
    public interface IAppNotificationService
    {
        Task SendNotificationAsync(Guid userId, string title, string message, string link, string type);
    }
}

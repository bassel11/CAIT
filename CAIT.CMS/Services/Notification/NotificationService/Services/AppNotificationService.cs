using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Entities;
using NotificationService.Hubs;

namespace NotificationService.Services
{
    public class AppNotificationService : IAppNotificationService
    {
        private readonly NotificationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IPushNotificationService _pushService;
        private readonly ILogger<AppNotificationService> _logger;

        public AppNotificationService(
            NotificationDbContext context,
            IHubContext<NotificationHub> hubContext,
            IPushNotificationService pushService,
            ILogger<AppNotificationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _pushService = pushService;
            _logger = logger;
        }

        public async Task SendNotificationAsync(Guid userId, string title, string message, string link, string type)
        {
            // 1. التخزين في قاعدة البيانات (للأرشيف وعرضه لاحقاً)
            var notification = new AppNotification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Link = link,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AppNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            // 2. الإرسال اللحظي عبر SignalR (إذا كان المستخدم متصلاً)
            // نستخدم User(userId) لإرسال الرسالة لشخص محدد فقط
            //await _hubContext.Clients.User(userId.ToString())
            //    .SendAsync("ReceiveNotification", new
            //    {
            //        Title = title,
            //        Message = message,
            //        Link = link,
            //        Type = type
            //    });

            // استخدم هذا السطر مؤقتاً للاختبار:
            await _hubContext.Clients.Group(userId.ToString().ToLower()) // نرسل للمجموعة التي انضم إليها
                .SendAsync("ReceiveNotification", new
                {
                    Title = title,
                    Message = message,
                    Link = link,
                    Type = type
                });

            // 3. ✅ الإرسال للموبايل (FCM Push Notification)
            try
            {
                // جلب التوكنات من الكاش أو الداتابيز
                var deviceTokens = await _context.UserDeviceTokens
                    .AsNoTracking() // أسرع للقراءة
                    .Where(x => x.UserId == userId)
                    .Select(x => x.DeviceToken)
                    .ToListAsync();

                if (deviceTokens.Any())
                {
                    var dataPayload = new Dictionary<string, string>
                    {
                        { "link", link },
                        { "type", type },
                        { "notificationId", notification.Id.ToString() }
                    };

                    await _pushService.SendMulticastAsync(deviceTokens, title, message, dataPayload);
                }
            }
            catch (Exception ex)
            {
                // يجب ألا يؤثر فشل الموبايل على باقي العملية
                _logger.LogError(ex, "Failed to dispatch push notification for user {UserId}", userId);
            }
        }
    }
}

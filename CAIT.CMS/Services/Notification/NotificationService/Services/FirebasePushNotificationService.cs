using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NotificationService.Services
{
    public interface IPushNotificationService
    {
        Task SendMulticastAsync(List<string> deviceTokens, string title, string body, Dictionary<string, string> data);
    }

    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly ILogger<FirebasePushNotificationService> _logger;

        public FirebasePushNotificationService(IConfiguration configuration, ILogger<FirebasePushNotificationService> logger)
        {
            _logger = logger;

            // Singleton Logic: تهيئة Firebase مرة واحدة
            if (FirebaseApp.DefaultInstance == null)
            {
                var path = configuration["Firebase:CredentialPath"] ?? "firebase-key.json";

                if (File.Exists(path))
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    });
                }
                else
                {
                    _logger.LogWarning("⚠️ Firebase credential file not found at: {path}. Push notifications will not work.", path);
                }
            }
        }

        public async Task SendMulticastAsync(List<string> deviceTokens, string title, string body, Dictionary<string, string> data)
        {
            if (deviceTokens == null || !deviceTokens.Any()) return;

            // إصلاح تحذير Dereference of a possibly null reference
            // نتأكد أن Firebase مهيأ قبل الاستخدام
            if (FirebaseApp.DefaultInstance == null)
            {
                _logger.LogError("FirebaseApp is not initialized.");
                return;
            }

            try
            {
                // تقسيم القائمة لأن Firebase يقبل 500 كحد أقصى
                var batches = deviceTokens.Chunk(500);

                foreach (var batch in batches)
                {
                    var message = new MulticastMessage()
                    {
                        Tokens = batch.ToList(), // تحويل لـ List لأن Chunk يرجع Array
                        Notification = new Notification()
                        {
                            Title = title,
                            Body = body
                        },
                        Data = data
                    };

                    // ✅ الإصلاح: استخدام الدالة الجديدة بدلاً من القديمة
                    // SendEachForMulticastAsync هي البديل الحديث لـ SendMulticastAsync
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                    _logger.LogInformation("🔥 FCM Sent: Success {success}, Failure {failure}", response.SuccessCount, response.FailureCount);

                    // (اختياري) هنا يمكن معالجة التوكنات الفاسدة وحذفها من الداتابيز إذا فشل الإرسال لها
                    if (response.FailureCount > 0)
                    {
                        foreach (var resp in response.Responses.Where(r => !r.IsSuccess))
                        {
                            _logger.LogWarning("FCM Failure: {error}", resp.Exception.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send FCM notification.");
            }
        }
    }
}
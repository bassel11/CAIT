using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Consumers.SendEmail;
using NotificationService.Data;
using NotificationService.Services;

namespace NotificationService
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. إعدادات الإيميل (Email Service)
            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.AddScoped<IEmailService, EmailService>();

            // 2. إعداد الداتابيز (ضرورية للـ Outbox/Inbox)
            var connectionString = configuration.GetConnectionString("NotificationConnectionString");
            services.AddDbContext<NotificationDbContext>(options =>
            {
                // إذا لم ترغب بإنشاء داتابيز حقيقية للإشعارات، يمكنك استخدام UseInMemoryDatabase
                // لكن للأمان والموثوقية يفضل SQL Server
                options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string is missing"));
            });

            // 3. إعداد MassTransit الموحد 🚀
            // نمرر الـ DbContext لتفعيل الـ Outbox
            services.AddMessageBroker<NotificationDbContext>(
                configuration,
                typeof(MoMApprovedNotificationConsumer).Assembly // اكتشاف الـ Consumer والـ Definition تلقائياً
            );

            return services;
        }
    }
}
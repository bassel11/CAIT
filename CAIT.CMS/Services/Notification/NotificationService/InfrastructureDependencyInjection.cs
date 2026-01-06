using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationService.Consumers.SendEmail;
using NotificationService.Data;
using NotificationService.Jobs;
using NotificationService.Services;
using Quartz;

namespace NotificationService
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. إعداد الداتابيز (ضرورية للـ Outbox/Inbox)
            var connectionString = configuration.GetConnectionString("NotificationConnectionString");
            services.AddDbContext<NotificationDbContext>(options =>
            {
                // إذا لم ترغب بإنشاء داتابيز حقيقية للإشعارات، يمكنك استخدام UseInMemoryDatabase
                // لكن للأمان والموثوقية يفضل SQL Server
                options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string is missing"));
            });


            // 2. إعدادات البريد
            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
            services.AddScoped<IEmailService, EmailService>();

            // 3. خدمات الإشعارات الجديدة
            services.AddScoped<IPushNotificationService, FirebasePushNotificationService>(); // خدمة FCM
            services.AddScoped<IAppNotificationService, AppNotificationService>();   // الخدمة الموحدة

            // 4. إعداد Quartz (للـ Cleanup Job)
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("NotificationCleanupJob");
                q.AddJob<NotificationCleanupJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("NotificationCleanup-Trigger")
                    .WithCronSchedule("0 5 9 * * ?")); // يعمل الساعة 3 فجراً يومياً
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            // 5. إعداد MassTransit الموحد 🚀
            // نمرر الـ DbContext لتفعيل الـ Outbox
            services.AddMessageBroker<NotificationDbContext>(
                configuration,
                typeof(MoMApprovedNotificationConsumer).Assembly // اكتشاف الـ Consumer والـ Definition تلقائياً
            );

            // 6. SignalR & CORS
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }
    }
}
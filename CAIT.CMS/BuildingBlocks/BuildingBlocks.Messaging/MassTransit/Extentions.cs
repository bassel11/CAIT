using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMessageBroker<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null,
            Action<IEntityFrameworkOutboxConfigurator>? configureOutbox = null) // مرونة إضافية
            where TContext : DbContext
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                    config.AddConsumers(assembly);

                // إعداد Outbox الموحد
                config.AddEntityFrameworkOutbox<TContext>(o =>
                {
                    o.UseBusOutbox();
                    //o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                    //o.DisableInboxCleanupService();

                    //زيادة النافذة لمنع التكرار حتى مع التأخير الطويل
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);

                    //  تفعيل التنظيف التلقائي  DisableInboxCleanupService)
                    // نضبط التأخير فقط لتخفيف الحمل على القاعدة
                    o.QueryDelay = TimeSpan.FromSeconds(30);


                    // استخدام SQL Server كافتراضي، إلا إذا تم تمرير إعداد آخر
                    if (configureOutbox != null)
                        configureOutbox(o);
                    else
                        o.UseSqlServer();
                });

                config.UsingRabbitMq((context, cfg) =>
                {
                    // ✅ الإصلاح 3: دعم VirtualHost لعزل بيئات Azure (Dev/Prod)
                    var host = configuration["RabbitMQ:Host"] ?? "localhost";
                    var vHost = configuration["RabbitMQ:VirtualHost"] ?? "/";

                    cfg.Host(host, vHost, h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });

                    // ✅ الإصلاح 4: إضافة سياسة إعادة المحاولة (ضروري لـ Azure)
                    cfg.UseMessageRetry(r =>
                        r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));

                    // ✅ الإصلاح 5: قاطع الدائرة لحماية النظام عند تعطل RabbitMQ
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(5);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // ✅ الإصلاح 6: إجبار التطبيق على انتظار الاتصال قبل البدء (Health Check)
            //services.AddOptions<MassTransitHostOptions>()
            //    .Configure(options =>
            //    {
            //        options.WaitUntilStarted = true;
            //        options.StartTimeout = TimeSpan.FromSeconds(30);
            //    });

            return services;
        }
    }
}
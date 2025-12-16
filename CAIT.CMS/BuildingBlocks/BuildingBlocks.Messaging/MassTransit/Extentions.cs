using MassTransit;
using Microsoft.EntityFrameworkCore; // ضروري للوصول لـ DbContext
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit
{
    public static class Extensions
    {
        // 1. نضيف <TContext> لتعريف الدالة
        // 2. نضيف شرط where TContext : DbContext لضمان الأمان
        public static IServiceCollection AddMessageBroker<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly? assembly = null)
            where TContext : DbContext
        {
            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                if (assembly != null)
                {
                    config.AddConsumers(assembly);
                }

                // 3. نمرر النوع TContext إلى دالة MassTransit
                config.AddEntityFrameworkOutbox<TContext>(o =>
                {
                    // ملاحظة: تأكد أن جميع المايكروسيرفس تستخدم نفس نوع قاعدة البيانات (SQL Server مثلاً)
                    // وإذا كانت مختلفة، يمكن تمرير إعدادات إضافية كـ Action
                    o.UseSqlServer();

                    o.UseBusOutbox();

                    o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                });

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });



                    // Configure all endpoints تلقائيًا لكل Consumer
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
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
                    o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
                    o.DisableInboxCleanupService();

                    // استخدام SQL Server كافتراضي، إلا إذا تم تمرير إعداد آخر
                    if (configureOutbox != null)
                        configureOutbox(o);
                    else
                        o.UseSqlServer();
                });

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
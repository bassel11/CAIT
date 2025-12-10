using Audit.Application.Contracts;
using Audit.Application.Repositories;
using Audit.Infrastructure.Consumers;
using Audit.Infrastructure.Repositories;
using Audit.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Audit.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // تسجيل الـ Repositories
            services.AddScoped<IAuditStore, AuditStore>();
            services.AddScoped<IAuditReadRepository, AuditReadRepository>();

            // MassTransit مع RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddConsumer<AuditEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });


                    cfg.ConfigureEndpoints(context);

                    //cfg.ReceiveEndpoint("audit-service-queue", e =>
                    //{
                    //    e.ConfigureConsumer<AuditEventConsumer>(context);
                    //});
                });
            });

            return services;
        }
    }
}
using Audit.Application.Authorization;
using Audit.Application.Contracts;
using Audit.Application.Repositories;
using Audit.Infrastructure.Authorization;
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
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Repositories
            services.AddScoped<IAuditStore, AuditStore>();
            services.AddScoped<IAuditReadRepository, AuditReadRepository>();

            // DelegatingHandler
            services.AddTransient<JwtDelegatingHandler>();

            // HttpClient for Permission Service
            var identityUrl = configuration["Services:IdentityBaseUrl"]
                ?? throw new InvalidOperationException("IdentityBaseUrl is not configured");

            services.AddHttpClient<IPermissionService, PermissionServiceHttpClient>(client =>
            {
                client.BaseAddress = new Uri(identityUrl);
            })
            .AddHttpMessageHandler<JwtDelegatingHandler>();


            // MassTransit
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

                    // Auto-create endpoints based on consumers
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}

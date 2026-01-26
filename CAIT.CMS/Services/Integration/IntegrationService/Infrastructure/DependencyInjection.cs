using BuildingBlocks.Messaging.MassTransit;
using IntegrationService.Application.Interfaces;
using IntegrationService.Infrastructure.Configuration;
using IntegrationService.Infrastructure.Persistence;
using IntegrationService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IntegrationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. إعدادات Azure AD
            services.Configure<AzureAdOptions>(configuration.GetSection("AzureAd"));

            // 2. قاعدة البيانات (IntegrationDB)
            services.AddDbContext<IntegrationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IntegrationConnectionString")
                // ,b => b.MigrationsAssembly(typeof(IntegrationDbContext).Assembly.FullName)
                ));

            // 3. خدمات التكامل (Graph)
            services.AddScoped<IMeetingPlatformService, MicrosoftGraphService>();

            // 4. إعداد MassTransit باستخدام الـ Extension الموحد الخاص بك
            // نمرر IntegrationDbContext ليتم تخزين الـ Outbox فيه
            services.AddMessageBroker<IntegrationDbContext>(
                configuration,
                Assembly.GetExecutingAssembly() // لاكتشاف الـ Consumers في هذا المشروع تلقائياً
            );

            return services;
        }
    }
}

using BuildingBlocks.Infrastructure;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskApplication.Common.Interfaces;
using TaskApplication.Data;
using TaskInfrastructure.Data.Interceptors;
using TaskInfrastructure.Persistence.Repositories;

namespace TaskInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // 1. تسجيل البنية التحتية الأساسية (User, HttpContext)
            services.AddSharedInfrastructure();

            // 2. ✅ تسجيل نظام التصاريح (الجديد)
            services.AddDynamicPermissions();

            // 3. ✅ تسجيل خدمة الاتصال بالهوية (للتحقق من الصلاحيات)
            // يجب أن تتأكد من وجود هذا الرابط في appsettings.json
            var identityUrl = configuration["Services:IdentityBaseUrl"];
            if (!string.IsNullOrEmpty(identityUrl))
            {
                services.AddRemotePermissionService(identityUrl);
            }

            // 4. تسجيل Interceptors
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<AuditPublishingInterceptor>();
            services.AddScoped<DispatchDomainEventsInterceptor>();

            // 5. إعداد DbContext
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                var auditInterceptor = sp.GetRequiredService<AuditPublishingInterceptor>();
                var dispatchInterceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();

                var connectionString = configuration.GetConnectionString("TaskConnectionString");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'TaskConnectionString' is missing.");
                }

                options.UseSqlServer(connectionString)
                        .AddInterceptors(
                            auditableInterceptor,
                            auditInterceptor,
                            dispatchInterceptor
                        );
            });

            // 6. ربط الواجهة
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<ITaskRepository, TaskRepository>();

            // 7. إعداد MassTransit
            services.AddMessageBroker<ApplicationDbContext>(
                configuration,
                typeof(TaskApplication.ApplicationDependencyInjection).Assembly
            );

            return services;
        }
    }
}

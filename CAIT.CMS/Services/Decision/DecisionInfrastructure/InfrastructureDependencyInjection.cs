using BuildingBlocks.Infrastructure; // ✅ هام جداً لاستدعاء الامتدادات
using BuildingBlocks.Messaging.MassTransit;
using DecisionApplication.Data;
using DecisionInfrastructure.Data.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DecisionInfrastructure
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
                //services.AddRemotePermissionService(identityUrl);
                services.AddRemotePermissionService(
                    identityUrl,
                    configuration: configuration,
                    serviceName: "Decision");
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

                var connectionString = configuration.GetConnectionString("DecisionConnectionString");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'DecisionConnectionString' is missing.");
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

            // 7. إعداد MassTransit
            services.AddMessageBroker<ApplicationDbContext>(
                configuration,
                typeof(DecisionApplication.ApplicationDependencyInjection).Assembly
            );

            return services;
        }
    }
}
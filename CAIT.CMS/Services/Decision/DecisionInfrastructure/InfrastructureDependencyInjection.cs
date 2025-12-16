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
            // 1. تسجيل Interceptors (ممتاز)
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<DispatchDomainEventsInterceptor>();

            // 2. إعداد DbContext (ممتاز - هذا هو الإعداد الآمن)
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                var dispatchInterceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();

                var connectionString = configuration.GetConnectionString("DecisionConnectionString");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'DecisionConnectionString' is missing.");
                }

                options.UseSqlServer(connectionString)
                       .AddInterceptors(auditableInterceptor, dispatchInterceptor);
            });

            // 3. ربط الواجهة بالكلاس (ممتاز - يمنع ازدواجية الاتصال)
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // 4. إعداد MassTransit (التعديل هنا: نستخدم الـ Extension الموحد)
            // هذا السطر يغنيك عن كتابة 30 سطر كود يدوياً
            services.AddMessageBroker<ApplicationDbContext>(
                configuration,
                typeof(DecisionApplication.ApplicationDependencyInjection).Assembly // نمرر اسم الأسمبلي ليتعرف على Consumers
            );

            return services;
        }
    }
}
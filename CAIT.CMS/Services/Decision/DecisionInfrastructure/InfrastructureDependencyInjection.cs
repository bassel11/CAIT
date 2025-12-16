using DecisionApplication.Data;
using DecisionInfrastructure.Data.Interceptors;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DecisionInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DecisionConnectionString");

            // 1. تسجيل Interceptors كـ كلاسات محددة (Concrete Classes) وليس واجهات عامة
            // هذا يضمن أننا نستطيع طلبهم بالاسم لاحقاً
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<DispatchDomainEventsInterceptor>();

            // 2. إعداد DbContext بدقة
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // أ) استدعاء الـ Interceptors المحددة بالاسم
                // هذا يضمن الترتيب ويمنع حقن interceptors غريبة من مكتبات أخرى
                var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                var dispatchInterceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();

                // ب) جلب والتحقق من نص الاتصال (Fail Fast)
                var connectionString = configuration.GetConnectionString("DecisionConnectionString");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'DecisionConnectionString' is missing.");
                }

                // ج) إضافة الإعدادات والـ Interceptors
                options.UseSqlServer(connectionString)
                       .AddInterceptors(auditableInterceptor, dispatchInterceptor);
                // ملاحظة: الترتيب هنا مهم، Auditable يملأ البيانات، ثم Dispatch ينشر الأحداث
            });

            // 3. ربط الواجهة بالكلاس (للاستخدام داخل الـ Handlers)
            // يفضل استخدام هذا النمط لضمان استخدام نفس الـ Instance الذي أنشأه AddDbContext
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            //services.AddMessageBroker<ApplicationDbContext>(
            //    configuration,
            //    Assembly.GetExecutingAssembly());

            //services.AddMessageBroker<ApplicationDbContext>(
            //    configuration,
            //    typeof(DecisionApplication.ApplicationDependencyInjection).Assembly // <--- التعديل هنا
            //);

            services.AddMassTransit(config =>
            {
                config.SetKebabCaseEndpointNameFormatter();

                // إضافة المستهلكين من طبقة التطبيق
                config.AddConsumers(typeof(DecisionApplication.ApplicationDependencyInjection).Assembly);

                // ====================================================
                // إعداد Outbox بشكل صريح لـ ApplicationDbContext
                // ====================================================
                config.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    // إعداد القفل (Locking) لـ SQL Server
                    o.UseSqlServer();

                    // هذا هو السطر السحري الذي يحول الـ PublishEndpoint
                    o.UseBusOutbox();

                    // مفيد جداً للتطوير لرؤية الجداول
                    o.DisableInboxCleanupService();
                    o.QueryDelay = TimeSpan.FromSeconds(10);
                });

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });

                    // تفعيل الـ Outbox لجميع الـ Endpoints
                    cfg.ConfigureEndpoints(context);
                });
            });


            return services;
        }
    }
}

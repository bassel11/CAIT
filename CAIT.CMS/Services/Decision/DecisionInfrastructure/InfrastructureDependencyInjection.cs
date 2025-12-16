using DecisionApplication.Data;
using DecisionInfrastructure.Data.Interceptors;
using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DecisionInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DecisionConnectionString");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                //options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>());
                var interceptors = sp.GetServices<ISaveChangesInterceptor>();
                options.AddInterceptors(interceptors);

                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

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

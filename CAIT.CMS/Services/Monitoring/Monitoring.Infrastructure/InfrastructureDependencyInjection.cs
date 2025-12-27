using BuildingBlocks.Infrastructure;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monitoring.Application.Data;
using Monitoring.Infrastructure.Data;
using Monitoring.Infrastructure.Jobs;
using Quartz;

namespace Monitoring.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1️⃣ Shared / Cross-cutting infrastructure
            services.AddSharedInfrastructure();

            // 2️⃣ Dynamic permissions (RBAC / ABAC)
            services.AddDynamicPermissions();

            // 3️⃣ Identity / Authorization integration
            var identityBaseUrl = configuration.GetValue<string>("Services:IdentityBaseUrl");

            if (!string.IsNullOrWhiteSpace(identityBaseUrl))
            {
                services.AddRemotePermissionService(identityBaseUrl);
            }

            // 4️⃣ Monitoring Database (Read / Analytics / Monitoring DB)
            services.AddDbContext<MonitoringDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("MonitoringConnectionString"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });


            services.AddScoped<IMonitoringDbContext>(provider
                => provider.GetRequiredService<MonitoringDbContext>());

            services.AddMessageBroker<MonitoringDbContext>(
               configuration,
               typeof(Monitoring.Application.ApplicationDependencyInjection).Assembly
           );

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("ComplianceCheckJob");
                q.AddJob<ComplianceCheckJob>(opts => opts.WithIdentity(jobKey));

                // تشغيل كل يوم الساعة 2 صباحاً
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("ComplianceCheck-Trigger")
                    .WithCronSchedule("0 0 2 * * ?"));
            });

            // التأكد من تشغيل Quartz كخدمة مستضافة (Hosted Service)
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}

using BuildingBlocks.Infrastructure;
using BuildingBlocks.Messaging.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TaskApplication.Common.Interfaces;
using TaskApplication.Data;
using TaskInfrastructure.Data.Interceptors;
using TaskInfrastructure.Jobs;
using TaskInfrastructure.Persistence.Repositories;
using TaskInfrastructure.Services;

namespace TaskInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
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
                services.AddRemotePermissionService(
                    identityUrl,
                    configuration: configuration,
                    serviceName: "task:");
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

            var useAzure = configuration.GetValue<bool>("Storage:UseAzure");

            if (useAzure)
            {
                // الخيار السحابي
                //services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            }
            else
            {
                // الخيار المحلي
                services.AddScoped<IFileStorageService, LocalStorageService>();
            }

            // 7. إعداد MassTransit
            services.AddMessageBroker<ApplicationDbContext>(
                configuration,
                typeof(TaskApplication.ApplicationDependencyInjection).Assembly
            );

            // =========================================================
            // 8. ✅ تسجيل مهام الأتمتة والجدولة (Quartz.NET) - جديد
            // =========================================================
            //services.AddQuartz(q =>
            //{
            //    // استخدام DI Factory للسماح بحقن MediatR داخل الـ Jobs
            //    //q.UseMicrosoftDependencyInjectionJobFactory();

            //    // أ) وظيفة التصعيد (تعمل كل ساعة للتحقق من المهام المتأخرة)
            //    var escalationJobKey = new JobKey("TaskEscalationJob");
            //    q.AddJob<TaskEscalationJob>(opts => opts.WithIdentity(escalationJobKey));

            //    q.AddTrigger(opts => opts
            //        .ForJob(escalationJobKey)
            //        .WithIdentity("TaskEscalationTrigger")
            //        .WithSimpleSchedule(x => x
            //            .WithIntervalInHours(1) // تحقق كل ساعة
            //            .RepeatForever()));

            //    // ب) وظيفة التذكيرات (تعمل مرة يومياً الساعة 8 صباحاً)
            //    var reminderJobKey = new JobKey("TaskReminderJob");
            //    q.AddJob<TaskReminderJob>(opts => opts.WithIdentity(reminderJobKey));

            //    q.AddTrigger(opts => opts
            //        .ForJob(reminderJobKey)
            //        .WithIdentity("TaskReminderTrigger")
            //        .WithCronSchedule("0 0 8 * * ?")); // صيغة Cron: الساعة 08:00 صباحاً يومياً
            //});



            // في TaskInfrastructure/DependencyInjection.cs

            services.AddQuartz(q =>
            {
                var quartzSettings = configuration.GetSection("QuartzSettings");

                // تحقق من القيمة قبل التسجيل
                if (quartzSettings.GetValue<bool>("EnableEscalationJob"))
                {
                    var escalationJobKey = new JobKey("TaskEscalationJob");
                    q.AddJob<TaskEscalationJob>(opts => opts.WithIdentity(escalationJobKey));
                    q.AddTrigger(opts => opts
                        .ForJob(escalationJobKey)
                        .WithIdentity("TaskEscalationTrigger")
                        .StartNow()
                        .WithSimpleSchedule(x => x.WithIntervalInSeconds(60).RepeatForever()));
                }

                if (quartzSettings.GetValue<bool>("EnableReminderJob"))
                {
                    var reminderJobKey = new JobKey("TaskReminderJob");
                    q.AddJob<TaskReminderJob>(opts => opts.WithIdentity(reminderJobKey));
                    q.AddTrigger(opts => opts
                        .ForJob(reminderJobKey)
                        .WithIdentity("TaskReminderTrigger")
                        .StartNow()
                        .WithSimpleSchedule(x => x.WithIntervalInSeconds(60).RepeatForever()));
                }
            });

            // التأكد من تشغيل Quartz كخدمة مستضافة (Hosted Service)
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }
    }
}

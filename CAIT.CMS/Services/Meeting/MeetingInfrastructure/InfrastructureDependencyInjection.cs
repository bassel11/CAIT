using BuildingBlocks.Infrastructure;
using BuildingBlocks.Messaging.MassTransit;
using BuildingBlocks.Shared.Abstractions;
using MassTransit;
using MeetingApplication.Data;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingApplication.Interfaces.AI;
using MeetingApplication.Interfaces.Scheduling;
using MeetingApplication.Wrappers;
using MeetingCore.DomainServices;
using MeetingCore.Repositories;
using MeetingInfrastructure.Audit;
using MeetingInfrastructure.Data;
using MeetingInfrastructure.DomainServices;
using MeetingInfrastructure.Integrations;
using MeetingInfrastructure.Interceptors;
using MeetingInfrastructure.Pdf;
using MeetingInfrastructure.Repositories;
using MeetingInfrastructure.Services;
using MeetingInfrastructure.Services.AI;
using MeetingInfrastructure.Services.Scheduling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // مهم لإضافة IConfiguration
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace MeetingInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // تسجيل المستخدم والـ HttpContext
            services.AddSharedInfrastructure();

            // تسجيل نظام التصاريح (Policy Provider + Handler)
            services.AddDynamicPermissions();

            // تسجيل خدمة الاتصال بالهوية (للتحقق من الصلاحيات)
            var identityUrl = configuration["Services:IdentityBaseUrl"];
            if (!string.IsNullOrEmpty(identityUrl))
            {
                //services.AddRemotePermissionService(identityUrl);
                services.AddRemotePermissionService(
                    identityUrl,
                    configuration: configuration,
                    serviceName: "meeting:");
            }

            // 1. تسجيل Generic Audit Handler (لأن MediatR لا يراه تلقائياً في Assembly آخر)
            // services.AddTransient(typeof(INotificationHandler<>), typeof(AuditDomainEventHandler<>));

            // 2. تسجيل Interceptor
            services.AddScoped<DispatchDomainEventsInterceptor>();
            services.AddScoped<AuditPublishingInterceptor>();

            // 3. إعداد DB Context (مع التحقق الصارم من نص الاتصال)
            services.AddDbContext<MeetingDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<DispatchDomainEventsInterceptor>();
                var auditInterceptor = sp.GetRequiredService<AuditPublishingInterceptor>();

                // البحث عن نص الاتصال
                var connectionString = configuration.GetConnectionString("MeetingConnectionString")
                                       ?? configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'MeetingConnectionString' is missing in appsettings.json.");
                }

                options.UseSqlServer(connectionString)
                       .AddInterceptors(interceptor, auditInterceptor);
            });


            services.AddScoped<IMeetingDbContext>(
                provider => provider.GetRequiredService<MeetingDbContext>());


            // Repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<IMinutesRepository, MinutesRepository>();
            services.AddScoped<IAgendaTemplateRepository, AgendaTemplateRepository>();
            services.AddScoped<IAiAgendaService, MockAiAgendaService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMinutesAIService, MinutesAIService>();

            // Services
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IPaginationService, PaginationService>();

            // PDF & Storage
            services.AddSingleton<IPdfGenerator, SimpleHtmlPdfGenerator>();
            services.AddScoped<IStorageService, StorageService>();

            // Stubs
            services.AddSingleton<OutlookClientStub>();
            services.AddSingleton<BusPublisherStub>();
            services.AddScoped<IAuditService, AuditService>();

            services.AddScoped<IMeetingSchedulerGateway, QuartzMeetingSchedulerGateway>();
            // Domain Services
            services.AddScoped<IMeetingOverlapDomainService, MeetingOverlapDomainService>();

            services.AddQuartz(q =>
            {
                // التخزين الدائم في SQL Server
                q.UsePersistentStore(s =>
                {
                    s.UseProperties = true; // لتخزين البيانات كـ Key-Value
                    s.RetryInterval = TimeSpan.FromSeconds(15);

                    s.UseSqlServer(sqlServer =>
                    {
                        // 1. استخراج نص الاتصال في متغير
                        var connectionString = configuration.GetConnectionString("MeetingConnectionString")
                                               ?? configuration.GetConnectionString("DefaultConnection");

                        // 2. التحقق الصارم: إذا كان فارغاً، نرمي خطأ يوقف التطبيق فوراً
                        if (string.IsNullOrWhiteSpace(connectionString))
                        {
                            throw new InvalidOperationException("CRITICAL: Quartz Connection string is missing in appsettings.json.");
                        }

                        // 3. التعيين الآمن (الآن الكومبايلر يعرف أن القيمة ليست null)
                        sqlServer.ConnectionString = connectionString;
                        sqlServer.TablePrefix = "QRTZ_";
                    });

                    // استخدام JSON لتخزين بيانات الـ JobDataMap (أفضل للصيانة من Binary)
                    s.UseNewtonsoftJsonSerializer();
                });
            });

            // تشغيل Quartz كـ Hosted Service (يعمل مع بدء التطبيق)
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true; // الانتظار حتى تنتهي الوظائف عند الإغلاق
            });

            services.AddMessageBroker<MeetingDbContext>(
                configuration,
                typeof(MeetingInfrastructure.InfrastructureDependencyInjection).Assembly // تحديد الأسمبلي لاكتشاف الـ Consumers
            );
            // Hosted Service لتشغيل الـ Bus تلقائياً
            //services.AddMassTransitHostedService();

            return services;
        }
    }
}
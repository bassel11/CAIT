using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.EventHandlers;
using BuildingBlocks.Messaging.MassTransit;
using MassTransit;
using MediatR;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using MeetingInfrastructure.Audit;
using MeetingInfrastructure.Data;
using MeetingInfrastructure.Integrations;
using MeetingInfrastructure.Interceptors;
using MeetingInfrastructure.Pdf;
using MeetingInfrastructure.Repositories;
using MeetingInfrastructure.Services;
using MeetingInfrastructure.Services.DateTimeProvider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // مهم لإضافة IConfiguration
using Microsoft.Extensions.DependencyInjection;

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
            services.AddTransient(typeof(INotificationHandler<>), typeof(AuditDomainEventHandler<>));

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

                // 🛑 Fail Fast: إيقاف التطبيق إذا كان النص مفقوداً
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("CRITICAL: Connection string 'MeetingConnectionString' is missing in appsettings.json.");
                }

                options.UseSqlServer(connectionString)
                       .AddInterceptors(interceptor, auditInterceptor);
            });

            // Repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<IAgendaRepository, AgendaRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IMoMRepository, MoMRepository>();
            services.AddScoped<IMoMAttachmentRepository, MoMAttachmentRepository>();
            services.AddScoped<IMeetingNotificationRepository, MeetingNotificationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IEventBus, EventBus>(); // e.g., MassTransit or custom
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IPaginationService, PaginationService>();

            // PDF & Storage
            services.AddSingleton<IPdfGenerator, SimpleHtmlPdfGenerator>();
            // services.AddSingleton<IStorageService>(_ => new LocalFileStorageService("./storage"));
            services.AddScoped<IStorageService, StorageService>();

            // Stubs
            services.AddSingleton<OutlookClientStub>();
            services.AddSingleton<BusPublisherStub>();



            // Other integrations
            services.AddScoped<IOutlookService, OutlookService>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<IAuditService, AuditService>();


            // --------------------
            // MassTransit
            // --------------------
            // ========================
            // MassTransit + RabbitMQ
            // ========================
            //services.AddMassTransit(x =>
            //{
            //    // تسجيل جميع الـConsumers من Assembly
            //    //x.AddConsumers(typeof(ApproveMoMCommandHandler).Assembly);
            //    x.AddConsumers(typeof(OutlookAttachMoMConsumer).Assembly);

            //    // EF Core Outbox (Transactional Outbox)
            //    x.AddEntityFrameworkOutbox<MeetingDbContext>(o =>
            //    {
            //        o.UseSqlServer();     // أو UsePostgres
            //        o.UseBusOutbox();     // يضمن إرسال الرسائل بعد نجاح SaveChanges

            //        // 👇 هذا الخيار يمنع الحذف ويجعل الرسائل تبقى معلمة كـProcessed
            //        o.DisableInboxCleanupService(); // إذا أردت التحكم في التنظيف بنفسك
            //        o.QueryDelay = TimeSpan.FromSeconds(10);

            //    });

            //    // إعداد RabbitMQ
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
            //        {
            //            h.Username(configuration["RabbitMQ:User"] ?? "guest");
            //            h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
            //        });

            //        // Configure all endpoints تلقائيًا لكل Consumer
            //        cfg.ConfigureEndpoints(context);
            //    });
            //});



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
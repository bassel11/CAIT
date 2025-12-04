using MassTransit;
using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using MeetingInfrastructure.Audit;
using MeetingInfrastructure.Data;
using MeetingInfrastructure.Integrations;
using MeetingInfrastructure.Messaging.Consumers;
using MeetingInfrastructure.Pdf;
using MeetingInfrastructure.Repositories;
using MeetingInfrastructure.Services;
using MeetingInfrastructure.Services.DateTimeProvider;
using Microsoft.Extensions.Configuration; // مهم لإضافة IConfiguration
using Microsoft.Extensions.DependencyInjection;

namespace MeetingInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
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

            // Outbox
            //services.AddScoped<IOutboxRouter, OutboxRouter>();
            //services.AddScoped<IOutboxHandler, IntegrationOutboxHandler>();
            //services.AddScoped<IntegrationOutboxHandler>();
            //services.AddScoped<OutlookOutboxHandler>();
            //services.AddScoped<NotificationOutboxHandler>();
            //services.AddScoped<TeamsOutboxHandler>();
            //services.AddScoped<AuditOutboxHandler>();

            // RabbitMQ

            // Other integrations
            services.AddScoped<IOutlookService, OutlookService>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuditService, AuditService>();

            // Hosted service
            //services.AddHostedService<RabbitMqConsumer>(); for test consuming


            // --------------------
            // MassTransit
            // --------------------
            // ========================
            // MassTransit + RabbitMQ
            // ========================
            services.AddMassTransit(x =>
            {
                // تسجيل جميع الـConsumers من Assembly
                //x.AddConsumers(typeof(ApproveMoMCommandHandler).Assembly);
                x.AddConsumers(typeof(OutlookAttachMoMConsumer).Assembly);

                // EF Core Outbox (Transactional Outbox)
                x.AddEntityFrameworkOutbox<MeetingDbContext>(o =>
                {
                    o.UseSqlServer();     // أو UsePostgres
                    o.UseBusOutbox();     // يضمن إرسال الرسائل بعد نجاح SaveChanges

                    // 👇 هذا الخيار يمنع الحذف ويجعل الرسائل تبقى معلمة كـProcessed
                    o.DisableInboxCleanupService(); // إذا أردت التحكم في التنظيف بنفسك
                    o.QueryDelay = TimeSpan.FromSeconds(10);

                });

                // إعداد RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:User"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Pass"] ?? "guest");
                    });

                    // Configure all endpoints تلقائيًا لكل Consumer
                    cfg.ConfigureEndpoints(context);
                });
            });

            // Hosted Service لتشغيل الـ Bus تلقائياً
            //services.AddMassTransitHostedService();



            // ✅ إضافة إعدادات SMTP عبر Options Pattern
            services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));

            return services;
        }
    }
}
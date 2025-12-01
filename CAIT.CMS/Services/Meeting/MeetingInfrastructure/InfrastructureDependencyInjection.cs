using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Integrations;
using MeetingApplication.Interfaces;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using MeetingInfrastructure.Audit;
using MeetingInfrastructure.Integrations;
using MeetingInfrastructure.Outbox;
using MeetingInfrastructure.Pdf;
using MeetingInfrastructure.RabbitMQ;
using MeetingInfrastructure.Repositories;
using MeetingInfrastructure.Services;
using MeetingInfrastructure.Services.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
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
            services.AddScoped<IOutboxService, OutboxService>();
            services.AddScoped<IOutboxRouter, OutboxRouter>();
            services.AddScoped<IOutboxHandler, IntegrationOutboxHandler>();
            services.AddScoped<IntegrationOutboxHandler>();
            services.AddScoped<OutlookOutboxHandler>();
            services.AddScoped<NotificationOutboxHandler>();
            services.AddScoped<TeamsOutboxHandler>();
            services.AddScoped<AuditOutboxHandler>();

            // RabbitMQ
            services.AddSingleton<IMessageBus, RabbitMqBus>();

            // Other integrations
            services.AddScoped<IOutlookService, OutlookService>();
            services.AddScoped<ITeamsService, TeamsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAuditService, AuditService>();

            // Hosted service
            services.AddHostedService<OutboxProcessor>();

            return services;
        }
    }
}
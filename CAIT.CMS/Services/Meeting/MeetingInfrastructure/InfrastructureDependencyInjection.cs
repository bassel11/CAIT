using MeetingApplication.Common.DateTimeProvider;
using MeetingApplication.Interfaces.Integrations;
using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using MeetingInfrastructure.Integrations;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITeamsService, TeamsService>(); // implement in infra
            services.AddScoped<IOutlookService, OutlookService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEventBus, EventBus>(); // e.g., MassTransit or custom

            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            services.AddScoped<IPaginationService, PaginationService>();

            return services;
        }
    }
}

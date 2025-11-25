using MeetingApplication.Wrappers;
using MeetingCore.Repositories;
using MeetingInfrastructure.Repositories;
using MeetingInfrastructure.Services;
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<IPaginationService, PaginationService>();

            return services;
        }
    }
}

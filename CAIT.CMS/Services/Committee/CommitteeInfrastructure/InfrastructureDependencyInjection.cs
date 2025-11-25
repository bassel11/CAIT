using CommitteeApplication.Wrappers;
using CommitteeCore.Repositories;
using CommitteeInfrastructure.Repositories;
using CommitteeInfrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommitteeInfrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<ICommitteeRepository, CommitteeRepository>();
            services.AddScoped<ICommitteeMemberRepository, CommitteeMemberRepository>();
            services.AddScoped<ICommitteeMemberRoleRepository, CommitteeMemberRoleRepository>();
            services.AddScoped<IStatusHistoryRepository, StatusHistoryRepository>();
            services.AddScoped<ICommitteeQuorumRuleRepository, CommitteeQuorumRuleRepository>();

            // Services
            services.AddScoped<IPaginationService, PaginationService>();

            return services;
        }
    }
}

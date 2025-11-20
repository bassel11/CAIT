using CommitteeApplication.Behaviour;
using CommitteeApplication.Common.CurrentUser;
using CommitteeApplication.Features.StatusHistories.Commands.Handlers;
using CommitteeApplication.Features.StatusHistories.Commands.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommitteeApplication
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(AddCommitStatusHistoryCommandValidator).Assembly);

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddCommitStatusHistoryCommandHandler).Assembly));

            // AutoMapper
            services.AddAutoMapper(typeof(AddCommitStatusHistoryCommandValidator).Assembly); // أو أي Mapping Profile موجود في Application Layer

            // Pipeline Behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}

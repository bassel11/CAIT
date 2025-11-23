using FluentValidation;
using MediatR;
using MeetingApplication.Behaviour;
using MeetingApplication.Features.Meetings.Commands.Handlers;
using MeetingApplication.Features.Meetings.Commands.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace MeetingApplication
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // FluentValidation
            services.AddValidatorsFromAssembly(typeof(CreateMeetingValidator).Assembly);

            // MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateMeetingCommandHandler).Assembly));

            // AutoMapper
            services.AddAutoMapper(typeof(CreateMeetingValidator).Assembly); // أو أي Mapping Profile موجود في Application Layer

            // Pipeline Behaviours
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            //services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}

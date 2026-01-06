using BuildingBlocks.Messaging.MassTransit;
using Identity.Application.Interfaces;
using Identity.Application.Interfaces.Authorization;
using Identity.Application.Interfaces.Permissions;
using Identity.Application.Interfaces.RolePermissions;
using Identity.Application.Interfaces.Roles;
using Identity.Application.Interfaces.UserRoles;
using Identity.Application.Interfaces.Users;
using Identity.Application.Interfaces.UsrRolPermRes;
using Identity.Application.Security;
using Identity.Application.Security.SecurityEventPublisher;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Messaging;
using Identity.Infrastructure.Security;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Services.Authorization;
using Identity.Infrastructure.Services.Permissions;
using Identity.Infrastructure.Services.RolePermissions;
using Identity.Infrastructure.Services.Roles;
using Identity.Infrastructure.Services.UserRoles;
using Identity.Infrastructure.Services.Users;
using Identity.Infrastructure.Services.UsrRolPermRes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Http Context
            services.AddHttpContextAccessor();
            // Memory Cache
            services.AddMemoryCache();

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ILdapAuthService, LdapAuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMfaService, MfaService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAzureAuthService, AzureAuthService>();
            services.AddScoped<IAzureB2BService, AzureB2BService>();
            services.AddScoped<IGuestUserAsync, GuestUserAsync>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRolePremissionService, RolePremissionService>();
            services.AddScoped<IPermissionCacheInvalidator, PermissionCacheInvalidator>();
            services.AddScoped<IUsrRolPermResService, UsrRolPermResService>();
            services.AddScoped<IPermissionSnapshotQuery, PermissionSnapshotQuery>();
            services.AddScoped<IPermissionSnapshotService, PermissionSnapshotService>();

            // Security
            services.AddScoped<ILoginSecurityService, LoginSecurityService>();
            services.AddScoped<ISecurityEventPublisher, SecurityEventPublisher>();


            // MassTransit + Outbox
            services.AddMessageBroker<ApplicationDbContext>(
                configuration,
                typeof(Identity.Application.ApplicationDependencyInjection).Assembly
            );
            return services;
        }
    }
}

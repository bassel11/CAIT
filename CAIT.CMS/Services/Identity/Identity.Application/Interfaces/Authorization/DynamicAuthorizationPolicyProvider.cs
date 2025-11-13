using Identity.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Identity.Application.Interfaces.Authorization
{
    public class DynamicAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX = "Permission:";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var permissionName = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permissionName))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
            _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}

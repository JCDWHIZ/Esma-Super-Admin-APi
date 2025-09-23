using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authorization;

public class PermissionProvider : IAuthorizationPolicyProvider
{
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    public PermissionProvider(IOptions<AuthorizationOptions> options)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        FallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
        {
            string permission = policyName["Permission:".Length..];
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}

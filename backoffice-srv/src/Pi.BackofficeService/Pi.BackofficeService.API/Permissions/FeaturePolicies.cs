using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions;

public abstract class FeaturePolicies
{
    public abstract string FeatureName { get; set; }
    protected abstract IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public IDictionary<string, AuthorizationPolicy> GetPolicies()
    {
        return Policies.ToDictionary(key => FeatureName + key.Key, value => value.Value);
    }

    protected static AuthorizationPolicy BuildPolicy(params string[] allowedValues)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole(allowedValues)
            .Build();
    }
}

using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class CuratedManagerPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "CuratedManager";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public CuratedManagerPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Access", BuildPolicy("curated-manager")},
        };
    }
}
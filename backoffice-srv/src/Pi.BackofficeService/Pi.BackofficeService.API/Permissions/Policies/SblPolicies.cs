using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class SblPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "Sbl";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public SblPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Read", BuildPolicy("sbl-read")},
            {"Edit", BuildPolicy("sbl-edit")},
        };
    }
}

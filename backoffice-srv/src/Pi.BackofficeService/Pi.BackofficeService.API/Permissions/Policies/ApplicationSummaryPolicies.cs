using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class ApplicationSummaryPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "ApplicationSummary";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public ApplicationSummaryPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Access", BuildPolicy("application-summary")},
        };
    }
}

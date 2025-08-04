using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class DocumentPortalPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "DocumentPortal";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public DocumentPortalPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Access", BuildPolicy("application-summary")},
        };
    }
}

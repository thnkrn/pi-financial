using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class OcrPortalPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "OcrPortal";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public OcrPortalPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Access", BuildPolicy("ocr-portal")},
        };
    }
}

using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class ReportPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "Report";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public ReportPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Read", BuildPolicy("report-read")},
            {"Export", BuildPolicy("report-export")},
        };
    }
}

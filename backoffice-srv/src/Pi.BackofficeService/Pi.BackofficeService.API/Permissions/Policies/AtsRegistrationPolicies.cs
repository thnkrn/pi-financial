using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class AtsRegistrationPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "AtsRegistration";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public AtsRegistrationPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Access", BuildPolicy("ats-registration")},
        };
    }
}

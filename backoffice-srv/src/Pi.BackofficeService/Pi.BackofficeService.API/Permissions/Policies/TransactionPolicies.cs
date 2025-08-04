using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class TransactionPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "Transaction";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public TransactionPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"Read", BuildPolicy("transaction-read")},
            {"Export", BuildPolicy("transaction-export")},
        };
    }
}

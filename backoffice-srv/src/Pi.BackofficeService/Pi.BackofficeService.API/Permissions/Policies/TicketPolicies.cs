using Microsoft.AspNetCore.Authorization;

namespace Pi.BackofficeService.API.Permissions.Policies;

public class TicketPolicies : FeaturePolicies
{
    public override string FeatureName { get; set; } = "Ticket";
    protected sealed override IDictionary<string, AuthorizationPolicy> Policies { get; set; }

    public TicketPolicies()
    {
        Policies = new Dictionary<string, AuthorizationPolicy>()
        {
            {"IndexRead", BuildPolicy("ticket-workspace-view")},
            {"Read", BuildPolicy("ticket-read")},
            {"Write", BuildPolicy("ticket-manage")},
            {"Edit", BuildPolicy("ticket-manage")},
        };
    }
}

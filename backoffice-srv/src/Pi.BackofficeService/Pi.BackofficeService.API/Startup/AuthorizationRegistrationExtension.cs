using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Pi.BackofficeService.API.Permissions.Policies;

namespace Pi.BackofficeService.API.Startup;

public static class AuthorizationRegistrationExtension
{
    public static IServiceCollection SetupPermissions(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.SetupAuthentication(configuration);
        services.SetupAuthorization(configuration);

        return services;
    }

    public static IServiceCollection SetupAuthorization(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicies(new TicketPolicies().GetPolicies());
            options.AddPolicies(new TransactionPolicies().GetPolicies());
            options.AddPolicies(new DocumentPortalPolicies().GetPolicies());
            options.AddPolicies(new OcrPortalPolicies().GetPolicies());
            options.AddPolicies(new ApplicationSummaryPolicies().GetPolicies());
            options.AddPolicies(new ReportPolicies().GetPolicies());
            options.AddPolicies(new SblPolicies().GetPolicies());
            options.AddPolicies(new AtsRegistrationPolicies().GetPolicies());
            options.AddPolicies(new CuratedManagerPolicies().GetPolicies());
        });

        return services;
    }

    public static IServiceCollection SetupAuthentication(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration.GetValue<string>("Authentication:Authority");
                options.Audience = configuration.GetValue<string>("Authentication:Audience");
                options.RequireHttpsMetadata = false; // Remove on prod
                options.ClaimsIssuer = "";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetValue<string>("Authentication:Issuer"),
                    ValidAudience = configuration.GetValue<string>("Authentication:Audience"),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                };
            });

        return services;
    }

    public static string? GetSubjectClaim(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static Guid? GetIamId(this ClaimsPrincipal user)
    {
        var subject = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return subject != null ? Guid.Parse(subject) : null;
    }

    public static AuthorizationOptions AddPolicies(this AuthorizationOptions options, IDictionary<string, AuthorizationPolicy> policies)
    {
        foreach (var policy in policies)
        {
            options.AddPolicy(policy.Key, policy.Value);
        }

        return options;
    }
}

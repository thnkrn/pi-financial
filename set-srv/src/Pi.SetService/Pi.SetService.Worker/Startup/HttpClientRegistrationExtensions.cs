using Microsoft.Extensions.Options;
using Pi.SetService.Infrastructure.Handlers;
using Pi.SetService.Infrastructure.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Pi.SetService.Worker.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("Onboard")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<OnboardServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("User")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<UserServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("Market")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddHttpMessageHandler<HttpMessageLoggingHandler>()
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<MarketServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("OnePortDb2")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<OnePortDb2ServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });
        services.AddHttpClient("OnePortTcp")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<OnePortTcpServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("PiInternal")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<PiInternalServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("Notification")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<NotificationServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });
        services.AddHttpClient("Bond")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<BondMarketServiceOptions>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("UserV2")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler((sp, _) =>
            {
                var options = sp.GetRequiredService<IOptionsSnapshot<UserServiceV2Options>>();
                var delay = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), options.Value.MaxRetry);
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(delay)
                    .WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
            });

        services.AddHttpClient("GrowthBook")
            .ConfigureHttpClient((_, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty); });

        return services;
    }
}

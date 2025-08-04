using Microsoft.Extensions.Options;
using Pi.Financial.Client.Freewill;
using Pi.Financial.FundService.Infrastructure.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Pi.Financial.FundService.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            services.AddHttpClient("Freewill")
                .ConfigureHttpClient((_, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Freewill:Host")!);
                    client.Timeout = TimeSpan.FromSeconds(2);
                })
                .AddHttpMessageHandler(() => new FreewillSecurityPolicyHandler(
                    configuration.GetValue<string>("Freewill:Requester")!,
                    configuration.GetValue<string>("Freewill:Application")!,
                    configuration.GetValue<string>("Freewill:Keybase")!,
                    configuration.GetValue<string>("Freewill:IvCode")!))
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        // TODO: Check Develop
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) => true
                    });

            services.AddHttpClient("GrowthBook")
                .ConfigureHttpClient((_, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty); });

            services.AddHttpClient("Onboard")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<OnboardServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });

            services.AddHttpClient("FundConnext")
                .ConfigureHttpClient((_, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("FundConnext:Host")!); });

            services.AddHttpClient("User")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<UserServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });

            services.AddHttpClient("Market")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<MarketServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });

            services.AddHttpClient("ItBackoffice")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<ItBackofficeOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });

            return services;
        }
    }
}

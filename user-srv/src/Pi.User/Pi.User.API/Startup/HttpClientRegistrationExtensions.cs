using System;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.Freewill;
using Pi.User.Application.Options;
using Pi.User.Infrastructure.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Pi.User.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
        {
            services.AddHttpClient("Sirius")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) }))
                .AddHeaderPropagation();
            services.AddHttpClient("Freewill")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddHttpMessageHandler((sp) =>
                {
                    var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                    return new FreewillSecurityPolicyHandler(
                        options.Value.Requester,
                        options.Value.Application,
                        options.Value.KeyBase,
                        options.Value.IvCode);
                })
                .ConfigurePrimaryHttpMessageHandler((cc) =>
                {
                    if (environment.IsDevelopment())
                    {
                        return new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            ServerCertificateCustomValidationCallback =
                                        (httpRequestMessage, cert, cetChain, policyErrors) => true
                        };
                    }
                    else
                    {
                        return new HttpClientHandler();
                    }
                })
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<FreewillOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                })
                .AddHeaderPropagation();

            services.AddHttpClient("Onboard")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
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


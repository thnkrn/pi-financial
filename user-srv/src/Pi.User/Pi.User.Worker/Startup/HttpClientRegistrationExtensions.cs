using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.Freewill;
using Pi.User.Application.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Pi.User.Worker.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
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
                });
            services.AddHttpClient("Sirius")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Sirius:Host") ?? string.Empty); });

            services.AddHttpClient("Pdf")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Pdf:Host")!); })
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
                });

            return services;
        }
    }
}


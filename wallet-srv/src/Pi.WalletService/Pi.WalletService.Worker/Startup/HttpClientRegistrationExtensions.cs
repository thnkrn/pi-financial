using System;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Security;
using Pi.WalletService.Infrastructure.Options;
using Polly.Extensions.Http;
using Polly;
using Pi.Client.CgsBank;
using Pi.Financial.Client.Freewill;
using Polly.Timeout;
using Polly.Contrib.WaitAndRetry;

namespace Pi.WalletService.Worker.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
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
            services.AddHttpClient("Otp")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<OtpServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });
            services.AddHttpClient("Payment")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<PaymentServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });
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
            services.AddHttpClient("CgsBank")
                .AddHttpMessageHandler((sp) =>
                {
                    var options = sp.GetRequiredService<IOptions<CgsBankServiceOptions>>();
                    return new CgsBankSecurityHandler(options.Value.SecretKey);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler((sp, _) =>
                {
                    var options = sp.GetRequiredService<IOptionsSnapshot<CgsBankServiceOptions>>();
                    var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: options.Value.MaxRetry);
                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>()
                        .WaitAndRetryAsync(delay).WrapAsync(Policy.TimeoutAsync(TimeSpan.FromMilliseconds(options.Value.TimeoutMS)));
                });
            services.AddHttpClient("Freewill")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Freewill:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Freewill:Timeout"));
                })
                .AddHttpMessageHandler(() => new FreewillSecurityPolicyHandler(
                    configuration.GetValue<string>("Freewill:Requester") ?? string.Empty,
                    configuration.GetValue<string>("Freewill:Application") ?? string.Empty,
                    configuration.GetValue<string>("Freewill:Keybase") ?? string.Empty,
                    configuration.GetValue<string>("Freewill:IvCode") ?? string.Empty))
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        // TODO: Check Develop
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) => true
                    });
            services.AddHttpClient("Sirius")
                .ConfigureHttpClient((sp, client) => { client.Timeout = TimeSpan.FromSeconds(30); })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddTransientHttpErrorPolicy(builder =>
                    builder.WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    }));

            services.AddHttpClient("GrowthBook")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty);
                });
            services.AddHttpClient("Settrade")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Settrade:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Settrade:Timeout"));
                });
            services.AddHttpClient("Notification")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Notification:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Notification:Timeout"));
                });
            services.AddHttpClient("Employee")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Employee:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Employee:Timeout"));
                });
            return services;
        }
    }
}

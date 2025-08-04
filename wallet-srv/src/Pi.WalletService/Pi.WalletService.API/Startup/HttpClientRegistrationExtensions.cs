using Pi.Financial.Client.Freewill;
using Microsoft.Extensions.Options;
using Pi.Client.CgsBank;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Infrastructure.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly.Contrib.WaitAndRetry;

namespace Pi.WalletService.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpClient("ExanteUserManagement")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Exante:UserManagementHost") ?? string.Empty); }).AddHeaderPropagation();
            services.AddHttpClient("ExanteTrade")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Exante:TradeHost") ?? string.Empty); }).AddHeaderPropagation();
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
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("Freewill:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("Freewill:Timeout"));
                })
                .AddHttpMessageHandler((sp) =>
                {
                    var options = sp.GetRequiredService<IOptions<FreewillOptions>>();
                    return new FreewillSecurityPolicyHandler(
                        options.Value.Requester,
                        options.Value.Application,
                        options.Value.KeyBase,
                        options.Value.IvCode);
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
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        // TODO: Check Develop
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) => true
                    });

            services.AddHttpClient("Fx").ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Fx:Host") ?? string.Empty); });

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
                })
                .AddHeaderPropagation();

            return services;
        }
    }
}

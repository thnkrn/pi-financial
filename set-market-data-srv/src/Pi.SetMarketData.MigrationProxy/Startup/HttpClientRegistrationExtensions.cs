using Polly;
using Pi.SetMarketData.MigrationProxy.ConstantConfigurations;

namespace Pi.SetMarketData.MigrationProxy.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddHttpClient("SETClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.SETBaseURL)
                        ?? throw new InvalidCastException("SETBaseURL is not configured");
                    client.BaseAddress = new Uri(url);
                }
            )
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
            );

        services
            .AddHttpClient("GEClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.GEBaseURL)
                        ?? throw new InvalidCastException("GEBaseURL is not configured");
                    client.BaseAddress = new Uri(url);
                }
            )
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
            );

        services
            .AddHttpClient("CommonClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.CommonBaseURL)
                        ?? throw new InvalidCastException("CommonBaseURL is not configured");
                    client.BaseAddress = new Uri(url);
                }
            )
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
            );

        services
            .AddHttpClient("SiriusClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.SiriusBaseURL)
                        ?? throw new InvalidCastException("SiriusBaseURL is not configured");
                    client.BaseAddress = new Uri(url);
                }
            )
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
            );

        services
           .AddHttpClient("GrowthBook")
           .ConfigureHttpClient(
               (_, client) =>
               {
                   client.BaseAddress = new Uri(
                       configuration.GetValue<string>("GrowthBook:Host")!
                   );
                   client.Timeout = TimeSpan.FromSeconds(30);
               }
           )
           .SetHandlerLifetime(TimeSpan.FromMinutes(5))
           .AddTransientHttpErrorPolicy(
               builder =>
                   builder.WaitAndRetryAsync(
                       new[]
                       {
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(10)
                       }
                   )
           );

        return services;
    }
}
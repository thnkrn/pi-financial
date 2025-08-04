using Pi.GlobalMarketData.DataMigrationConsumer.Constants;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Polly;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Example init HTTP Client
        //
        // services.AddHttpClient("Freewill")
        //     .ConfigureHttpClient((sp, client) =>
        //     {
        //         // TODO: add config for freewill host, timeout and use polly retry policy
        //         // client.BaseAddress = hostContext.Configuration.GetValue<string>("Freewill:Host");
        //     });
        //// .AddHttpMessageHandler(() => new FreewillSecurityPolicyHandler(configuration.GetValue<string>("RabbitMQ:Password"))); // TODO: change to proper config

        // services.AddHttpClient("FundConnext")
        //     .ConfigureHttpClient((sp, client) =>
        //     {
        //         // TODO: add config for fundconnext host, timeout and use polly retry policy
        //         // client.BaseAddress = hostContext.Configuration.GetValue<string>("FundConnext:Host");
        //     });

        services
            .AddHttpClient(HttpClientKeys.VelexaHttpApi)
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiBaseUrl) ?? ""
                    );
                }
            )
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                )
            );

        return services;
    }
}
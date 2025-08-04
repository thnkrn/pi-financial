using System;
namespace Pi.FundMarketData.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
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

            return services;
        }
    }
}


using Polly;

namespace Pi.TfexService.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpClient("GrowthBook")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty);
                });
            services.AddHttpClient("SetTrade")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("SetTrade:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("SetTrade:Timeout"));
                })
                .AddHeaderPropagation();
            services.AddHttpClient("MarketData")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("MarketData:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("MarketData:Timeout"));
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

            return services;
        }
    }
}


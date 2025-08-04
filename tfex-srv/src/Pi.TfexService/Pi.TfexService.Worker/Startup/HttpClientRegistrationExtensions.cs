using Polly;

namespace Pi.TfexService.Worker.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("GrowthBook")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty);
                });
            services.AddHttpClient("UserService")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("UserService:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("UserService:Timeout"));
                });
            services.AddHttpClient("NotificationService")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("NotificationService:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("NotificationService:Timeout"));
                });
            services.AddHttpClient("SetTrade")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("SetTrade:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("SetTrade:Timeout"));
                });
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
using System;
namespace Pi.TfexService.Listener.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("SetTrade")
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("SetTrade:Host") ?? string.Empty);
                    client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("SetTrade:Timeout"));
                });

            return services;
        }
    }
}


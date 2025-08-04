namespace Pi.BackofficeService.API.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpClient("ActivityService")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("ActivityService:Host") ?? string.Empty); });
            services.AddHttpClient("WalletService")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("WalletService:Host") ?? string.Empty); });
            services.AddHttpClient("Lambda")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Lambda:Host") ?? string.Empty); });
            services.AddHttpClient("OcrHttpClient")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("OcrService:Host") ?? string.Empty); });
            services.AddHttpClient("SblService")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("SblService:Host") ?? string.Empty); });
            services.AddHttpClient("CuratedManagerHttpClient")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("CuratedManagerService:Host") ?? string.Empty); });
            return services;
        }
    }
}


namespace Pi.BackofficeService.Worker.Startup
{
    public static class HttpClientRegistrationExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("WalletService")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("WalletService:Host") ?? string.Empty); });
            services.AddHttpClient("Lambda")
                .ConfigureHttpClient((sp, client) => { client.BaseAddress = new Uri(configuration.GetValue<string>("Lambda:Host") ?? string.Empty); });

            return services;
        }
    }
}


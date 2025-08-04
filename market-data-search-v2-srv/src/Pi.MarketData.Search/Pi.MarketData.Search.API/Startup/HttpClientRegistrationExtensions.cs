using Polly;
using System.Net.Http.Headers;

namespace Pi.MarketData.Search.API.Startup;

public static class HttpClientRegistrationExtensions
{
    private static void ConfigureClientBaseAddress(HttpClient client, string url)
    {
        client.BaseAddress = new Uri(url);
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var userFavoriteConfig = configuration.GetSection("UserFavorite");
        services
            .AddHttpClient("UserFavoriteClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = userFavoriteConfig["BaseUrl"] ?? throw new InvalidCastException("UserFavoriteBaseURL is not configured");
                    ConfigureClientBaseAddress(client, url);
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


using System.Net.Http.Headers;
using Pi.MarketData.Domain.ConstantConfigurations;
using Polly;

namespace Pi.MarketData.MigrationProxy.API.Startup;

public static class HttpClientRegistrationExtensions
{
    private static readonly HttpClient ValidationClient = new() { Timeout = TimeSpan.FromSeconds(5) };

    public static IServiceCollection AddHttpClients(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .AddHttpClient("SETClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.SetBaseUrl)
                              ?? throw new InvalidCastException("SETBaseURL is not configured");

                    ConfigureClientBaseAddress(client, url);
                }
            );

        services
            .AddHttpClient("GEClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.GeBaseUrl)
                              ?? throw new InvalidCastException("GEBaseURL is not configured");

                    ConfigureClientBaseAddress(client, url);
                }
            );

        services
            .AddHttpClient("CommonClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.CommonBaseUrl)
                              ?? throw new InvalidCastException("CommonBaseURL is not configured");

                    ConfigureClientBaseAddress(client, url);
                }
            );

        services
            .AddHttpClient("SiriusClient")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.SiriusBaseUrl)
                              ?? throw new InvalidCastException("SiriusBaseURL is not configured");

                    ConfigureClientBaseAddress(client, url);
                }
            );

        services
            .AddHttpClient("SearchV2Client")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (_, client) =>
                {
                    var url = configuration.GetValue<string>(ConfigurationKeys.SearchV2BaseUrl)
                              ?? throw new InvalidCastException("SiriusBaseURL is not configured");

                    ConfigureClientBaseAddress(client, url);
                }
            );

        services
            .AddHttpClient("GrowthBook")
            .ConfigureHttpClient(
                (_, client) =>
                {
                    client.BaseAddress = new Uri(configuration.GetValue<string>("GrowthBook:Host") ?? string.Empty);
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

    private static void ConfigureClientBaseAddress(HttpClient client, string url)
    {
        client.BaseAddress = new Uri(UrlAccessible(url));
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
    }

    private static string UrlAccessible(string url)
    {
        try
        {
            string urlAccessible;

            if (!url.StartsWith($"{Uri.UriSchemeHttp}://", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith($"{Uri.UriSchemeHttps}://", StringComparison.OrdinalIgnoreCase))
                urlAccessible = $"{Uri.UriSchemeHttp}://{url}";
            else return url;

            var httpResponse = ValidationClient.Send(new HttpRequestMessage(HttpMethod.Head, urlAccessible));
            if (!httpResponse.IsSuccessStatusCode)
            {
                urlAccessible = $"{Uri.UriSchemeHttps}://{url}";

                var httpsResponse = ValidationClient.Send(new HttpRequestMessage(HttpMethod.Head, urlAccessible));
                if (!httpsResponse.IsSuccessStatusCode)
                    throw new InvalidOperationException($"Invalid or unreachable URL: {url}");
            }

            return urlAccessible;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Invalid or unreachable URL: {url}", ex);
        }
    }
}

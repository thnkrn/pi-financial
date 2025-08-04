using System.Net.Http.Headers;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;

namespace Pi.SetMarketDataRealTime.DataHandler.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("HolidayApi", client =>
        {
            var baseUrl = configuration.GetValue(ConfigurationKeys.SettradeHolidayApiBaseUrl.TrimEnd('/'),
                string.Empty);

            if (!string.IsNullOrEmpty(baseUrl)) client.BaseAddress = new Uri(baseUrl);

            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
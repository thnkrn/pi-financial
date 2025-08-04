using System.Net;
using Microsoft.Extensions.Logging;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Helpers;

public class HttpRequestHelper<T>(HttpClient httpClient, ILogger<T> logger)
{
    public async Task<HttpResponseMessage?> RequestByUrl(
        string url,
        HttpMethod? method = null,
        SecurityProtocolType? securityProtocolType = null
    )
    {
        try
        {
            var uri = new Uri(httpClient.BaseAddress + url);
            ServicePointManager.SecurityProtocol =
                securityProtocolType ?? SecurityProtocolType.Tls12;
            HttpRequestMessage requestMessage =
                new() { RequestUri = uri, Method = method ?? HttpMethod.Get };

            var response = await httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error on URL: {url}");
            return null;
        }
    }
}
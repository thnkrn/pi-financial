using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;

namespace Pi.SetMarketData.Infrastructure.Helpers;

public class HttpRequestHelper<T>(HttpClient httpClient, ILogger<T> logger) : IHttpRequestHelper
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
            logger.LogError(ex, "Error on URL: {URL}", url);
            return null;
        }
    }

    public async Task<HttpResponseMessage?> RequestByContent(
        string url,
        StringContent content,
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
                new()
                {
                    RequestUri = uri,
                    Method = method ?? HttpMethod.Get,
                    Content = content,
                    Headers =
                    {
                        Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                    }
                };

            var response = await httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on URL: {URL}", url);
            return null;
        }
    }
}
using System.Net;
using System.Text;
using Pi.SetMarketData.MigrationProxy.Interfaces;

namespace Pi.SetMarketData.MigrationProxy.Helpers;

public class HttpRequestHelper : IHttpRequestHelper
{
    private const SecurityProtocolType DefaultSecurityProtocol = SecurityProtocolType.Tls12;

    public virtual async Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context)
    {
        var method = new HttpMethod(context.Request.Method);
        var fullUrl = BuildFullUrl(context);
        var requestBody = await ReadRequestBody(context);

        return await RequestByUrl(httpClient, fullUrl, requestBody, method);
    }

    public static async Task<HttpResponseMessage?> RequestByUrl(
        HttpClient httpClient,
        string url,
        string? requestBody,
        HttpMethod method,
        SecurityProtocolType securityProtocolType = DefaultSecurityProtocol)
    {
        var uri = new Uri(new Uri(httpClient.BaseAddress?.ToString() ?? string.Empty), url);
        ServicePointManager.SecurityProtocol = securityProtocolType;

        using var requestMessage = new HttpRequestMessage
        {
            RequestUri = uri,
            Method = method
        };

        if (ShouldAddRequestBody(method, requestBody))
        {
            requestMessage.Content = new StringContent(requestBody!, Encoding.UTF8, "application/json");
        }

        return await httpClient.SendAsync(requestMessage);
    }

    private static string BuildFullUrl(HttpContext context)
    {
        var trimmedPath = context.Request.Path.Value ?? string.Empty;
        return $"{trimmedPath}{context.Request.QueryString}";
    }

    private static async Task<string> ReadRequestBody(HttpContext context)
    {
        if (context.Request.ContentLength <= 0)
        {
            return string.Empty;
        }

        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var requestBody = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return requestBody;
    }

    private static bool ShouldAddRequestBody(HttpMethod method, string? requestBody)
    {
        return !string.IsNullOrEmpty(requestBody) &&
               (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch);
    }
}
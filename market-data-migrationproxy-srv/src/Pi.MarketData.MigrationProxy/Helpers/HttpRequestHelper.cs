using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using Pi.MarketData.MigrationProxy.API.Options;

namespace Pi.MarketData.MigrationProxy.API.Helpers;

public class HttpRequestHelper : IHttpRequestHelper
{
    private const SecurityProtocolType DefaultSecurityProtocol = SecurityProtocolType.Tls12;
    private readonly ILogger<HttpRequestHelper> _logger;
    private readonly HttpForwarderOptions _options;

    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public HttpRequestHelper(IOptions<HttpForwarderOptions> options, ILogger<HttpRequestHelper> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context)
    {
        var method = new HttpMethod(context.Request.Method);
        var fullUrl = BuildFullUrl(context);
        var requestBody = await ReadRequestBody(context);
        var headers = BuildHeaders(context);

        return await RequestByUrl(httpClient, fullUrl, requestBody, method, headers);
    }

    public async Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context, string requestBody)
    {
        var method = new HttpMethod(context.Request.Method);
        var fullUrl = BuildFullUrl(context);
        var headers = BuildHeaders(context);

        return await RequestByUrl(httpClient, fullUrl, requestBody, method, headers);
    }

    public async Task<HttpResponseMessage?> RequestByUrl(
        HttpClient httpClient,
        string url,
        string? requestBody,
        HttpMethod method,
        Dictionary<string, string?[]>? headers = null,
        SecurityProtocolType securityProtocolType = DefaultSecurityProtocol,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(new Uri(httpClient.BaseAddress?.ToString() ?? string.Empty), url);
        _logger.LogDebug("Client {Method} on {Uri} (URI) and RequestBody is {RequestBody}", method.Method.ToUpper(),
            uri, requestBody);
        ServicePointManager.SecurityProtocol = securityProtocolType;

        using var requestMessage = new HttpRequestMessage();
        requestMessage.RequestUri = uri;
        requestMessage.Method = method;

        if (headers != null && headers.Count != 0)
        {
            foreach (var keyValue in headers)
            {
                requestMessage.Headers.Add(keyValue.Key, keyValue.Value);
            }
        }

        if (ShouldAddRequestBody(method, requestBody) && requestBody != null)
            requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage, cancellationToken);

        // The code below. Will be removed once the tests are finished.
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Response Content: {Content}", content);

        return response;
    }

    /// <summary>
    /// Performs a file upload request to the specified path
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for the request</param>
    /// <param name="path">The path to send the request to</param>
    /// <param name="file">The file to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The HTTP response from the request</returns>
    public async Task<HttpResponseMessage?> UploadFile
    (
        HttpClient httpClient,
        string path,
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        var url = path;
        if (!path.StartsWith('/'))
        {
            url = $"/{path}";
        }

        var uri = new Uri(new Uri(httpClient.BaseAddress?.ToString() ?? string.Empty), url);
        _logger.LogDebug("Uploading file {FileName} to {Uri}", file.FileName, uri);

        using var multipartContent = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream());

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        multipartContent.Add(fileContent, "file", file.FileName);

        _logger.LogDebug("Content-Type: {ContentType}", multipartContent.Headers.ContentType);

        return await httpClient.PostAsync(uri, multipartContent, cancellationToken);
    }

    private static string BuildFullUrl(HttpContext context)
    {
        var trimmedPath = context.Request.Path.Value ?? string.Empty;
        return $"{trimmedPath}{context.Request.QueryString}";
    }

    private static async Task<string> ReadRequestBody(HttpContext context)
    {
        if (context.Request.ContentLength <= 0) return string.Empty;

        context.Request.EnableBuffering();
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var requestBody = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return requestBody;
    }

    private Dictionary<string, string?[]> BuildHeaders(HttpContext context)
    {
        return context.Request.Headers.Where(h => _options.ForwardHeaders.Contains(h.Key))
            .ToDictionary(k => k.Key, v => v.Value.ToArray());
    }

    private static bool ShouldAddRequestBody(HttpMethod method, string? requestBody)
    {
        return !string.IsNullOrEmpty(requestBody) &&
               (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch);
    }
}

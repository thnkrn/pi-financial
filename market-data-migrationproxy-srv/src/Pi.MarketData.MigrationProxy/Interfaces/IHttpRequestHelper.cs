using System.Net;

namespace Pi.MarketData.MigrationProxy.API.Interfaces;

public interface IHttpRequestHelper
{
    Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context);
    Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context, string requestBody);
    Task<HttpResponseMessage?> RequestByUrl
    (
        HttpClient httpClient,
        string url,
        string? requestBody,
        HttpMethod method,
        Dictionary<string, string?[]>? headers = null,
        SecurityProtocolType securityProtocolType = SecurityProtocolType.Tls12,
        CancellationToken cancellationToken = default
    );
    Task<HttpResponseMessage?> UploadFile
    (
        HttpClient httpClient,
        string path,
        IFormFile file,
        CancellationToken cancellationToken = default
    );
}

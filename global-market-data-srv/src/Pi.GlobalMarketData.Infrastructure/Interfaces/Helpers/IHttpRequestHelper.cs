using System.Net;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;

public interface IHttpRequestHelper
{
    Task<HttpResponseMessage?> RequestByUrl(
        string url,
        HttpMethod? method = null,
        SecurityProtocolType? securityProtocolType = null
    );

    Task<HttpResponseMessage?> RequestByContent(
        string url,
        StringContent content,
        HttpMethod? method = null,
        SecurityProtocolType? securityProtocolType = null
    );
}
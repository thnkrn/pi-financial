namespace Pi.SetMarketData.MigrationProxy.Interfaces;

public interface IHttpRequestHelper
{
    Task<HttpResponseMessage?> Request(HttpClient httpClient, HttpContext context);
}

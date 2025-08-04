namespace Pi.SetMarketData.MigrationProxy.Interfaces;

public interface IHttpResponseHelper
{
    Task<HttpResponseMessage> CombineResponses(List<HttpResponseMessage> responses);
}

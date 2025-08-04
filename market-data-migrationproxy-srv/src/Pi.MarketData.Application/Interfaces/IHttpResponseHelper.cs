namespace Pi.MarketData.Application.Interfaces;

public interface IHttpResponseHelper
{
    Task<HttpResponseMessage> CombineResponses(List<HttpResponseMessage> responses);
}
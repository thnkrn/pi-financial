using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.TfexService.Application.Services.MarketData;

namespace Pi.TfexService.Infrastructure.Services;

public class MarketDataApi(HttpClient httpClient, string basePath) : IMarketDataApi
{
    private readonly SiriusApi _siriusApi = new SiriusApi(httpClient, basePath);

    public async Task<MarketTickerResponse> MarketTickerAsync(MarketTickerRequest marketTickerRequest)
    {
        var response = await _siriusApi.CgsV2MarketTickerPostAsync(string.Empty, marketTickerRequest);
        response.Response.Data.ForEach(x => x.InstrumentCategory = x.InstrumentCategory.Replace(" ", string.Empty));
        return response;
    }
}
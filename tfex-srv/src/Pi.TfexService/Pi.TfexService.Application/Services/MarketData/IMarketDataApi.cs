using Pi.Client.Sirius.Model;

namespace Pi.TfexService.Application.Services.MarketData;

// TODO: Migrate to MarketData Api Once Ready
// This solution works for now since AppSynth & Sirius share the same interfaces 
public interface IMarketDataApi
{
    Task<MarketTickerResponse> MarketTickerAsync(MarketTickerRequest marketTickerRequest);
}
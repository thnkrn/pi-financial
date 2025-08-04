using Pi.Client.Sirius.Model;

namespace Pi.TfexService.Application.Services.MarketData;

public interface IMarketDataService
{
    public Task<List<Ticker>> GetTicker(string? sid, List<string> symbols);
}
using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.GlobalMarketData.Model;
using Pi.Common.CommonModels;
using Pi.GlobalEquities.Application.Services.MarketData;
using Pi.GlobalEquities.DomainModels;

namespace Pi.GlobalEquities.Infrastructure.Services.MarketData;

public class MarketDataService : IMarketDataService
{
    private readonly IMarketDataApi _marketDataApi;
    private readonly ILogger<MarketDataService> _logger;

    public MarketDataService(IMarketDataApi marketDataApi, ILogger<MarketDataService> logger)
    {
        _marketDataApi = marketDataApi;
        _logger = logger;
    }

    public async Task<IEnumerable<SymbolPrice>> GetTicker(IEnumerable<string> symbols, CancellationToken ct)
    {
        var symbolsRequests = new List<PiGlobalMarketDataDomainModelsRequestMarketTickerParameter>();
        foreach (var sym in symbols)
        {
            if (string.IsNullOrEmpty(sym) || !sym.Contains('.'))
            {
                _logger.LogWarning("Invalid symbolId detected: {SymbolId}", sym);
                continue;
            }

            var symbolSplit = sym.Split(".");
            if (symbolSplit.Length != 2)
            {
                _logger.LogWarning("Invalid symbolId format: {SymbolId}", sym);
                continue;
            }
            symbolsRequests.Add(new PiGlobalMarketDataDomainModelsRequestMarketTickerParameter(
                symbol: symbolSplit[0], venue: symbolSplit[1]));
        }

        var request = new PiGlobalMarketDataDomainModelsRequestMarketTickerRequest(symbolsRequests);

        var marketTickerResult = await _marketDataApi.MarketTickerAsync(request, ct);

        return marketTickerResult.Response.Data.Select(sym => new SymbolPrice
        {
            Symbol = sym.Symbol,
            Venue = sym.Venue,
            Price = decimal.TryParse(sym.Price, CultureInfo.InvariantCulture, out var price) ? price : 0,
            Currency = Enum.TryParse<Currency>(sym.Currency, true, out var currency) ? currency : 0
        });
    }
}

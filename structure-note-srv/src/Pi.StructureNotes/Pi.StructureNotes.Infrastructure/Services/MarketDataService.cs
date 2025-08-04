using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.GlobalMarketData.Model;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;

namespace Pi.StructureNotes.Infrastructure.Services;

public class MarketDataService : IMarketDataService
{
    private readonly IMarketDataApi _marketDataApi;
    private readonly ISiriusApi _siriusApi;
    private ILogger<MarketDataService> _logger;

    public MarketDataService(IMarketDataApi marketDataApi, ISiriusApi siriusApi, ILogger<MarketDataService> logger)
    {
        _marketDataApi = marketDataApi;
        _siriusApi = siriusApi;
        _logger = logger;
    }

    public async Task<decimal> GetLastSessionClosePrice(string symbolId, CancellationToken ct)
    {
        try
        {
            var symbolIdParts = symbolId.Split('.');
            if (symbolIdParts.Length < 2)
                return 0;

            var venue = symbolIdParts[1];
            var symbol = symbolIdParts[0];

            var request = new PiGlobalMarketDataDomainModelsRequestMarketProfileOverviewRequest(symbol, venue);
            var result = await _marketDataApi.MarketProfileOverviewAsync(request, ct);

            var lastPriceResult = result?.Response?.LastPrice;

            return decimal.TryParse(lastPriceResult, out decimal lastPrice) ? lastPrice : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot get Last Price from Market Data API, {Symbol}", symbolId);
            return 0;
        }
    }

    public async Task<decimal> GetSiriusLastSessionClosePrice(string symbolId, CancellationToken ct)
    {
        var symbolIdParts = symbolId.Split('.');
        if (symbolIdParts.Length < 2)
            return 0;

        var venue = symbolIdParts[1];
        var symbol = symbolIdParts[0];
        var req = new MarketProfileOverviewRequest(symbol, venue);

        var result = await _siriusApi.CgsV1MarketProfileOverviewPostAsync(req, ct);
        decimal.TryParse(result?.Response?.LastPrice, out decimal lastPrice);

        return lastPrice;
    }

    public class ApiResponse<T>
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public T Response { get; set; }
        public string DebugStack { get; set; }
    }

    public class SiriusInstrumentProfileOverview
    {
        public string LastPrice { get; set; }
    }
}

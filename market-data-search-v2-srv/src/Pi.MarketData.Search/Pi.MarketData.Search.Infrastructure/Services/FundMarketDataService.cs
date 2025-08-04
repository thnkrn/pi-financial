using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi.Client.FundMarketData.Api;
using Pi.Client.FundMarketData.Model;
using Pi.MarketData.Search.Application.Services;
using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.Infrastructure.Services;

public class FundMarketDataService : IFundMarketDataService
{
    private readonly IFundApi _fundMarketDataApi;
    private readonly IDistributedCache _cache;
    private readonly ILogger<FundMarketDataService> _logger;

    public FundMarketDataService(IFundApi fundMarketDataApi, IDistributedCache cache, ILogger<FundMarketDataService> logger)
    {
        _fundMarketDataApi = fundMarketDataApi;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<InstrumentSummary>> GetFundSummaries(IEnumerable<string> fundCodes, CancellationToken ct)
    {
        try
        {
            const int chunkSize = 20;

            var tasks = fundCodes
                .Where(symbol => !string.IsNullOrWhiteSpace(symbol))
                .Select((symbol, index) => new { symbol, batch = index / chunkSize })
                .GroupBy(x => x.batch)
                .Select(batch => _fundMarketDataApi.InternalLegacyFundsSummariesPostAsync(
                    new PiFundMarketDataAPIModelsRequestsLegacySymbolsRequest(batch.Select(x => x.symbol).ToList()), ct));

            var results = await Task.WhenAll(tasks);

            var fundSummaries = results
                .SelectMany(result => result.Data.Select(res => new InstrumentSummary
                {
                    Venue = res.Venue,
                    Symbol = res.Symbol,
                    FriendlyName = res.FriendlyName,
                    Logo = res.Logo,
                    Price = res.Price,
                    PriceChangeRatio = (decimal)res.PriceChangeRatio,
                    Currency = res.Currency,
                    Type = InstrumentType.Fund,
                    Category = InstrumentCategory.Funds
                }));

            return fundSummaries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot Get Fund Summaries fundCodes: {fundCodes}", fundCodes);
            return Enumerable.Empty<InstrumentSummary>();
        }
    }

    public async Task<IEnumerable<InstrumentSummary>> GetTopFundsOver3Months(int limit = 30, CancellationToken ct = default)
    {
        const string cacheKey = "search::top_fund_over3months";
        var cacheValue = await _cache.GetStringAsync(cacheKey, ct);
        if (cacheValue != null)
        {
            var cache = JsonSerializer.Deserialize<IEnumerable<InstrumentSummary>>(cacheValue);
            if (cache != null)
            {
                return cache;
            }
        }

        var response = await _fundMarketDataApi.SecureFundsMarketBasketMarketSummariesV2GetAsync(
            PiFundMarketDataConstantsMarketBasket.TopFund,
            PiFundMarketDataConstantsInterval.Over3Months,
            null,
            limit,
            cancellationToken: ct);

        var result = response.Data.Data.Select(res => new InstrumentSummary
        {
            Venue = "Fund",
            Symbol = res.Symbol,
            FriendlyName = res.Name,
            Logo = res.AmcLogo,
            Price = res.Nav,
            PriceChangeRatio = res.ReturnPercentage ?? 0,
            Currency = res.Currency,
            Type = InstrumentType.Fund,
            Category = InstrumentCategory.Funds
        }).ToArray();

        if (result.Length != 0)
        {
            var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            await _cache.SetAsync(cacheKey, JsonSerializer.SerializeToUtf8Bytes(result), cacheOptions, ct);
        }

        return result;
    }

    public async Task<IEnumerable<InstrumentSummary>> SearchFunds(string keyword, CancellationToken ct)
    {
        try
        {
            var result = await _fundMarketDataApi.InternalFundsSearchGetAsync(keyword, ct);

            return result.Data.Select(x => new InstrumentSummary
            {
                Venue = "Fund",
                Symbol = x.Symbol,
                FriendlyName = x.Name,
                Logo = x.AmcLogo,
                Price = x.Nav,
                PriceChange = x.NavChange ?? 0,
                PriceChangeRatio = (decimal?)x.NavChangePercentage ?? 0,
                Currency = x.Currency,
                Type = InstrumentType.Fund,
                Category = InstrumentCategory.Funds
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot Search Fund keyword: {keyword}", keyword);
            return Enumerable.Empty<InstrumentSummary>();
        }
    }
}

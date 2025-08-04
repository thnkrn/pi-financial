using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Client.SetMarketData.Model;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Infrastructure.Factories;
using Pi.SetService.Infrastructure.Model;
using ListHelper = Pi.SetService.Infrastructure.Utils.ListHelper;
using MarketStatus = Pi.SetService.Domain.AggregatesModel.InstrumentAggregate.MarketStatus;
using ISetMarketDataApi = Pi.Client.SetMarketData.Api.IMarketDataApi;

namespace Pi.SetService.Infrastructure.Services;

public class MarketService(
    ILogger<UserService> logger,
    IDistributedCache cache,
    ISetMarketDataApi setMarketData,
    IOptions<SetTradingOptions> options) : IMarketService
{
    private const string Market = "SET";
    private const string EquityVenue = "Equity";

    public async Task<List<EquityInstrument>> GetEquityInstruments(string[] symbols,
        CancellationToken ct = default)
    {
        try
        {
            var symbolChunks = symbols.Length <= 30
                ? [symbols.ToList()]
                : ListHelper.SplitList(symbols, 30);

            var results = await Task.WhenAll(symbolChunks.Select(async chunk =>
            {
                try
                {
                    var instrumentInfoTasks = chunk.Select(async symbol =>
                        new MarketResponseWrapper<PiSetMarketDataDomainModelsResponseInstrumentInfoResponse>
                        {
                            Symbol = symbol,
                            Response = await GetInstrumentInfoAsync(symbol, ct)
                        }).ToList();

                    var profileOverviewTasks = chunk.Select(async symbol =>
                        new MarketResponseWrapper<PiSetMarketDataDomainModelsResponseProfileOverviewResponse>
                        {
                            Symbol = symbol,
                            Response = await GetProfileOverviewAsync(symbol, ct)
                        }).ToList();

                    var marketTickerTask = setMarketData.MarketTickerAsync(
                        new PiSetMarketDataDomainModelsRequestMarketTickerRequest
                        {
                            Param = chunk.Select(symbol => new PiSetMarketDataDomainModelsRequestMarketTickerParameter
                            { Symbol = symbol, Venue = EquityVenue }).ToList()
                        }, ct);

                    var instrumentInfos = await Task.WhenAll(instrumentInfoTasks);
                    var profileOverviews = await Task.WhenAll(profileOverviewTasks);
                    var marketTicker = await marketTickerTask;

                    var equityInstruments = new List<EquityInstrument>();
                    foreach (var symbol in chunk)
                    {
                        var instrumentInfo = instrumentInfos.FirstOrDefault(info => info.Symbol == symbol)?.Response;
                        var profileOverview = profileOverviews.FirstOrDefault(overview => overview.Symbol == symbol)
                            ?.Response;
                        var tickerData = marketTicker.Response.Data?.FirstOrDefault(t => t.Symbol == symbol);

                        if (instrumentInfo != null && profileOverview != null && tickerData != null)
                            equityInstruments.Add(MarketFactory.NewEquityInstrument(symbol, instrumentInfo,
                                profileOverview, tickerData));
                        else
                            logger.LogWarning("GetEquityInfosAsync data is empty for symbol: {Symbol}", symbol);
                    }

                    return equityInstruments;
                }
                catch (Exception e)
                {
                    logger.LogError(e, "GetEquityInfosAsync failed for chunk with error: {ErrMsg}", e.Message);
                    throw new InvalidOperationException(e.Message, e);
                }
            }));

            return results.SelectMany(result => result).ToList();
        }
        catch (Exception e)
        {
            throw new UnauthorizedAccessException(e.Message, e);
        }
    }

    public async Task<InstrumentProfile?> GetInstrumentProfile(string symbol, CancellationToken ct = default)
    {
        var apiResponse = await setMarketData.MarketTickerAsync(
            new PiSetMarketDataDomainModelsRequestMarketTickerRequest
            {
                Param =
                [
                    new PiSetMarketDataDomainModelsRequestMarketTickerParameter { Symbol = symbol, Venue = EquityVenue }
                ]
            }, ct);

        var ticker = apiResponse.Response.Data.FirstOrDefault();

        return ticker == null ? null : MarketFactory.NewInstrumentProfile(ticker);
    }

    public async Task<TradingDetail?> GetTradingDetail(string symbol, CancellationToken ct = default)
    {
        var apiResponse = await setMarketData.MarketTickerAsync(
            new PiSetMarketDataDomainModelsRequestMarketTickerRequest
            {
                Param =
                [
                    new PiSetMarketDataDomainModelsRequestMarketTickerParameter { Symbol = symbol, Venue = EquityVenue }
                ]
            }, ct);

        var ticker = apiResponse.Response.Data.FirstOrDefault();
        return ticker == null ? null : MarketFactory.NewTradingDetail(ticker);
    }

    public async Task<IEnumerable<CorporateAction>> GetCorporateActions(string symbol, CancellationToken ct = default)
    {
        var profileOverview = await GetProfileOverviewAsync(symbol, ct);

        if (profileOverview == null) return [];

        return (from action in profileOverview.CorporateActions
                where !string.IsNullOrEmpty(action.Type) && !string.IsNullOrEmpty(action.Date)
                select MarketFactory.NewCorporateAction(action.Type, action.Date)).ToList();
    }

    public async Task<CeilingFloor?> GetCeilingFloor(string symbol, CancellationToken ct = default)
    {
        var apiResponse = await setMarketData.MarketTickerAsync(
            new PiSetMarketDataDomainModelsRequestMarketTickerRequest
            {
                Param =
                [
                    new PiSetMarketDataDomainModelsRequestMarketTickerParameter { Symbol = symbol, Venue = EquityVenue }
                ]
            }, ct);

        var ticker = apiResponse.Response.Data.FirstOrDefault();
        if (ticker == null || ticker.Floor == "" || ticker.Ceiling == "") return null;

        return MarketFactory.NewCeilingFloor(ticker);
    }

    public async Task<MarketStatus> GetCurrentMarketStatus()
    {
        var response = await setMarketData.MarketStatusAsync(
            new PiSetMarketDataDomainModelsRequestMarketStatusRequest(Market));

        return MarketFactory.NewMarketStatus(options.Value, response.Response.MarketStatus);
    }

    public async Task<List<InstrumentProfile>> GetInstrumentsProfile(string[] symbols, CancellationToken ct = default)
    {
        try
        {
            var parameters = symbols
                .Select(symbol => new PiSetMarketDataDomainModelsRequestMarketTickerParameter
                { Symbol = symbol, Venue = EquityVenue })
                .ToList();

            var requestBody = new PiSetMarketDataDomainModelsRequestMarketTickerRequest
            {
                Param = parameters
            };

            try
            {
                var apiResponse = await setMarketData.MarketTickerAsync(requestBody, ct);

                if (apiResponse?.Response.Data == null || apiResponse.Response.Data.Count == 0)
                    return [];
                var instrumentProfiles = apiResponse.Response.Data
                    .Select(MarketFactory.NewInstrumentProfile)
                    .ToList();

                return instrumentProfiles;
            }
            catch (Exception e)
            {
                logger.LogError(e, "GetEquityInfosAsync for symbols \"{Symbols}\" failed with error {ErrMsg}",
                    string.Join(",", symbols), e.Message);
                return [];
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "GetEquityInfosAsync failed with error: {ErrMsg}", e.Message);
            throw new UnauthorizedAccessException(e.Message, e);
        }
    }

    private async Task<PiSetMarketDataDomainModelsResponseInstrumentInfoResponse> GetInstrumentInfoAsync(string symbol,
        CancellationToken ct)
    {
        var cacheKey = $"MarketInstrumentInfo_{symbol}";

        var cachedData = await cache.GetStringAsync(cacheKey, ct);
        if (cachedData is not null)
        {
            var data = JsonSerializer
                .Deserialize<PiSetMarketDataDomainModelsResponseInstrumentInfoResponse>(cachedData);
            if (data is not null) return data;
        }

        var response = await setMarketData.MarketInstrumentInfoAsync(
            new PiSetMarketDataDomainModelsRequestMarketInstrumentInfoRequest
            {
                Symbol = symbol,
                Venue = EquityVenue
            }, ct);

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(1));

        var serializedData = JsonSerializer.Serialize(response.Response);
        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions, ct);

        return response.Response;
    }

    private async Task<PiSetMarketDataDomainModelsResponseProfileOverviewResponse?> GetProfileOverviewAsync(
        string symbol, CancellationToken ct)
    {
        var cacheKey = $"MarketProfileOverview_{symbol}";

        var cachedData = await cache.GetStringAsync(cacheKey, ct);
        if (cachedData is not null)
        {
            var data =
                JsonSerializer.Deserialize<PiSetMarketDataDomainModelsResponseProfileOverviewResponse>(cachedData);
            if (data is not null) return data;
        }

        var response = await setMarketData.MarketProfileOverviewAsync(
            new PiSetMarketDataDomainModelsRequestMarketProfileOverviewRequest
            {
                Symbol = symbol,
                Venue = EquityVenue
            }, ct);

        if (response?.Response == null) return null;

        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(1));

        var serializedData = JsonSerializer.Serialize(response.Response);
        await cache.SetStringAsync(cacheKey, serializedData, cacheOptions, ct);

        return response.Response;
    }
}
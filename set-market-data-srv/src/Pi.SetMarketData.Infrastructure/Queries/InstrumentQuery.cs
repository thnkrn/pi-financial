using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Handlers.MarketTicker;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Queries;

public class InstrumentQuery(
    ICacheService cacheService,
    IEntityCacheService entityCacheService,
    MongoDbServices mongoDbService,
    IMongoService<CorporateAction> corporateActionService,
    ILogger<InstrumentQuery> logger) : IInstrumentQuery
{
    public async Task<IEnumerable<MarketStreamingResponse>> GetStreamingData(string[] symbols)
    {
        var result = new List<MarketStreamingResponse>();
        
        foreach (var symbol in symbols)
        {
            try
            {
                var instrument = await entityCacheService.GetInstrumentBySymbol(symbol);
                if (instrument == null)
                {
                    continue;
                }
            
                var marketStreaming = await cacheService.GetAsync<MarketStreamingResponse>($"{CacheKey.StreamingBody}{instrument.OrderBookId}");

                if (marketStreaming != null)
                {
                    result.Add(marketStreaming);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get Market Streaming for symbol: {Symbol}", symbol);
            }
        }
        
        return result;
    }

    public async Task<IEnumerable<InstrumentInfo>> GetInstrumentsInfo(InstrumentQueryParam[] queryParams)
    {
        if (queryParams.Length == 0) return [];
        
        var orderbooks = new Dictionary<int, (Instrument instrument, StreamingBody realtime, string venue)>();
        var underlyingOrderBookIDs = new HashSet<int>();

        var tasks = queryParams.Select(async queryParam =>
        {
            try
            {
                var instrument = await entityCacheService.GetInstrumentBySymbol(queryParam.Symbol);
                if (instrument == null)
                {
                    return (null, null, queryParam.Venue);
                }
        
                var marketStreaming = await cacheService.GetAsync<MarketStreamingResponse>($"{CacheKey.StreamingBody}{instrument.OrderBookId}");
                var realtime = marketStreaming?.Response.Data.FirstOrDefault(q => q.Symbol == queryParam.Symbol);
                
                return realtime == null ? (null, null, queryParam.Venue) : (instrument, realtime, queryParam.Venue);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to GetInstrumentsInfo for symbol: {Symbol}", queryParam.Symbol);
                return (null, null, queryParam.Venue);
            }
        });

        foreach (var item in await Task.WhenAll(tasks))
        {
            if (item.instrument == null || item.realtime == null)
            {
                continue;
            }
            
            if (item.instrument.UnderlyingOrderBookID != 0 && item.instrument.UnderlyingOrderBookID != null)
            {
                underlyingOrderBookIDs.Add((int)item.instrument.UnderlyingOrderBookID);
            }
            
            orderbooks[item.instrument.OrderBookId] = (item.instrument, item.realtime, item.Venue);
        }

        if (orderbooks.Count == 0)
        {
            return [];
        }
        
        var corporateActionsTask = corporateActionService.GetAllByFilterAsync(target =>
            target.OrderBookId != null && orderbooks.Keys.Contains((int)target.OrderBookId)
        );
        var instrumentsTask = Task.FromResult<IEnumerable<Instrument>>([]);
        
        if (underlyingOrderBookIDs.Count != 0)
        {
            instrumentsTask = mongoDbService.InstrumentService.GetAllByFilterAsync(t => underlyingOrderBookIDs.Contains(t.OrderBookId));
        }

        await Task.WhenAll(corporateActionsTask, instrumentsTask);
        
        var corporateActions = await corporateActionsTask;
        var underlyingLogoSymbols = (await instrumentsTask).ToDictionary(q => q.Symbol, q => (q.Logo, q.SecurityType));

        return orderbooks.Select(q =>
        {
            var instrument = q.Value.instrument;
            var realtime = q.Value.realtime;
            var (logoSymbol, securityType) = underlyingLogoSymbols.TryGetValue(instrument.Symbol, out var val) ? val : (null, null);
            if (string.IsNullOrEmpty(logoSymbol))
            {
                logoSymbol = instrument.Symbol;
                securityType = instrument.SecurityType;
            }
        
            // Might change to use from record but use this for the same logic as appsynth enpoint
            var logoMarket = string.IsNullOrEmpty(q.Value.venue) ? "SET" : q.Value.venue;
            var logo = LogoHelper.GetLogoUrl(logoMarket, securityType, logoSymbol);
            
            return new InstrumentInfo
            {
                Symbol = instrument.Symbol,
                Market = realtime.Market,
                MarketStatus = realtime.Status,
                Profile = new Profile
                {
                    Symbol = instrument.Symbol,
                    Logo = logo,
                    FriendlyName = instrument.FriendlyName,
                    InstrumentCategory = instrument.InstrumentCategory,
                },
                PricingDetail = new PricingDetail
                {
                    Price = realtime.Price,
                    PrevClose = realtime.PreClose,
                    Ceiling = realtime.Ceiling,
                    Floor = realtime.Floor,
                },
                CorporateActions = corporateActions.Where(x => x.OrderBookId == instrument.OrderBookId && !string.IsNullOrEmpty(x.Code))
                    .Select(x => new CorporateActionDetail
                        {
                            Date = x.Date,
                            Code = x.Code
                        })
            };
        });
    }
}

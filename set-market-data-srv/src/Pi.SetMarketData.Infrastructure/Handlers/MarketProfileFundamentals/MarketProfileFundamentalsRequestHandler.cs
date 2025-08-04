using Pi.SetMarketData.Application.Interfaces.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Services.MarketData.MarketProfileFundamentals;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketProfileFundamentals;

public class MarketProfileFundamentalsRequestHandler
    : GetMarketProfileFundamentalsRequestAbstractHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly ICacheService _cacheService;
    private readonly IMongoService<Domain.Entities.InstrumentDetail> _instrumentDetailService;
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    /// <summary>
    /// </summary>
    /// <param name="entityCacheService"></param>
    /// <param name="instrumentDetailService"></param>
    /// <param name="priceInfoService"></param>
    /// <param name="cacheService"></param>
    public MarketProfileFundamentalsRequestHandler(
        IEntityCacheService entityCacheService,
        IMongoService<Domain.Entities.InstrumentDetail> instrumentDetailService,
        IMongoService<Domain.Entities.PriceInfo> priceInfoService,
        ICacheService cacheService
    )
    {
        _entityCacheService = entityCacheService;
        _instrumentDetailService = instrumentDetailService;
        _priceInfoService = priceInfoService;
        _cacheService = cacheService;
    }

    protected override async Task<PostMarketProfileFundamentalsResponse> Handle(
        PostMarketProfileFundamentalsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var symbol = request.Data.Symbol ?? string.Empty;
            var venue = request.Data.Venue ?? string.Empty;

            var venueMapping = await _entityCacheService.GetSetVenueMapping(venue) ?? new SetVenueMapping();

            var exchangeId = venueMapping.ExchangeIdMs;

            var instrument = await _entityCacheService.GetInstrumentBySymbol(symbol) ?? new Domain.Entities.Instrument();

            var instrumentDetailResponse =
                await _cacheService.GetAsync<Domain.Entities.InstrumentDetail>($"{CacheKey.InstrumentDetail}{instrument.OrderBookId}")
                ?? new Domain.Entities.InstrumentDetail();

            var underlyingInstrument = new Domain.Entities.Instrument();

            if (instrumentDetailResponse.UnderlyingOrderBookID.HasValue)
            {
                underlyingInstrument = await _entityCacheService.GetInstrumentByOrderBookId(instrumentDetailResponse.UnderlyingOrderBookID.Value)
                    ?? new Domain.Entities.Instrument();
            }

            // Logo for response
            var logoMarket = underlyingInstrument.Venue ?? string.Empty;

            if (string.IsNullOrEmpty(logoMarket))
                logoMarket = "SET";

            underlyingInstrument.Logo = LogoHelper.GetLogoUrl(logoMarket,
                underlyingInstrument.SecurityType ?? string.Empty,
                underlyingInstrument.Symbol ?? string.Empty);

            var instrumentDetail = _entityCacheService.GetInstrumentDetailByOrderBookId(instrument.OrderBookId);

            var morningStarStocks =
                 _entityCacheService.GetMorningStarStocks(symbol, exchangeId ?? "");

            var marketStreamingResponse =
                 _cacheService.GetAsync<MarketStreamingResponse>(
                    $"{CacheKey.StreamingBody}{instrument.OrderBookId}"
                );

            var underlyingStreamingResponse = _cacheService.GetAsync<MarketStreamingResponse>(
                $"{CacheKey.StreamingBody}{instrumentDetailResponse.UnderlyingOrderBookID ?? 0}"
            );

            Task.WaitAll([
                instrumentDetail, morningStarStocks, marketStreamingResponse, underlyingStreamingResponse], 
                cancellationToken: cancellationToken
            );

            var result = new MarketProfileFundamentalsService()
                .SetVenue(venue)
                .SetMorningStar(await morningStarStocks)
                .SetInstrument(instrument, underlyingInstrument)
                .SetInstrumentDetail(await instrumentDetail)
                .SetMarketStreaming(await marketStreamingResponse)
                .SetUnderlying(await underlyingStreamingResponse)
                .GetResult();

            return new PostMarketProfileFundamentalsResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

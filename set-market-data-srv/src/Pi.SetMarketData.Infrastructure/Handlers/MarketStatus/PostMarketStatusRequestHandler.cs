using Pi.SetMarketData.Application.Interfaces.MarketStatus;
using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Application.Queries.MarketTicker;
using Pi.SetMarketData.Application.Services.MarketData.MarketStatus;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketStatus;

public class PostMarketTickerRequestHandler : PostMarketStatusRequestAbstractHandler
{
    private readonly ICacheService _cacheService;
    private readonly IMongoService<MarketCode> _marketCodeService;

    /// <summary>
    /// </summary>
    /// <param name="cacheService"></param>
    /// <param name="marketCodeService"></param>
    public PostMarketTickerRequestHandler(ICacheService cacheService, IMongoService<MarketCode> marketCodeService)
    {
        _cacheService = cacheService;
        _marketCodeService = marketCodeService;
    }

    protected override async Task<PostMarketStatusResponse> Handle(PostMarketStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var marketCode = await _marketCodeService.GetByFilterAsync(result =>
                result.Market != null && result.Market.Equals(request.Data.Market, StringComparison.OrdinalIgnoreCase)
            ) ?? new MarketCode();

            var marketStreaming = await _cacheService.GetAsync<Domain.Entities.MarketStatus>
                ($"{CacheKey.MarketStatus}{marketCode.OrderBookId}") 
                ?? new Domain.Entities.MarketStatus();

            var result = MarketStatusService.GetResult(marketStreaming);

            return new PostMarketStatusResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
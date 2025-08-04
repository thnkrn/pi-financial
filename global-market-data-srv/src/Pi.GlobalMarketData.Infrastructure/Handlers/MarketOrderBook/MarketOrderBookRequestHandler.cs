using Pi.GlobalMarketData.Application.Interfaces.MarketOrderBook;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketOrderBook;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Models.Response;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketOrderBook;

public class MarketOrderBookRequestHandler : PostOrderBookAbstractHandler
{
    private readonly ICacheService _cacheService;
    public MarketOrderBookRequestHandler(
        ICacheService cacheService
    )
    {
        _cacheService = cacheService;
    }

    protected override async Task<PostOrderBookResponse> Handle
    (
        PostOrderBookRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<StreamingBody> streamingBodyList = new List<StreamingBody>(request.Data.SymbolVenueList?.Count ?? 0);

            foreach (var symbolVenue in request.Data.SymbolVenueList)
            {
                var streamingBodyCache = await _cacheService.GetAsync<StreamingBody>(
                    $"{CacheKey.StreamingBody}{symbolVenue.Symbol}"
                ) ?? new StreamingBody
                {
                    Symbol = symbolVenue.Symbol,
                    Venue = symbolVenue.Venue
                };
                streamingBodyList.Add(streamingBodyCache);
            }
            var result = MarketOrderBookService.GetResult(streamingBodyList);

            return new PostOrderBookResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
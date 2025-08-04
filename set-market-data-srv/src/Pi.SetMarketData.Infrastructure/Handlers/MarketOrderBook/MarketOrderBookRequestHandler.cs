using Pi.SetMarketData.Application.Interfaces.MarketOrderBook;
using Pi.SetMarketData.Application.Queries.OrderBook;
using Pi.SetMarketData.Application.Services.MarketData.MarketOrderBook;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketOrderBook
{
    public class MarketOrderBookRequestHandler : PostOrderBookAbstractHandler
    {
        private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
        private readonly ICacheService _cacheService;

        public MarketOrderBookRequestHandler(
            IMongoService<Domain.Entities.Instrument> instrumentService,
            ICacheService cacheService
        )
        {
            _instrumentService = instrumentService;
            _cacheService = cacheService;
        }

        protected override async Task<PostOrderBookResponse> Handle(
            PostOrderBookRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                List<Domain.Entities.Instrument> instrumentsList = [];
                List<string> venue = [];
                List<MarketStreamingResponse> marketStreamingList = [];

                for (int i = 0; i < request.Data.SymbolVenueList?.Count; i++)
                {
                    var instrumentResponse =
                        await _instrumentService.GetByFilterAsync(target =>
                            target.Symbol == request.Data.SymbolVenueList[i].Symbol
                        ) ?? new Domain.Entities.Instrument();

                    var marketStreamingValue = await _cacheService.GetAsync<MarketStreamingResponse>(
                        $"{CacheKey.StreamingBody}{instrumentResponse.OrderBookId}"
                    ) ?? new MarketStreamingResponse();

                    instrumentsList.Add(instrumentResponse);
                    venue.Add(request.Data.SymbolVenueList[i].Venue ?? "");
                    marketStreamingList.Add(marketStreamingValue);
                }
                var result = MarketOrderBookService.GetResult(
                    instrumentsList,
                    venue,
                    marketStreamingList
                );

                return new PostOrderBookResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

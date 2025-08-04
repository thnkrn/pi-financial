using Pi.SetMarketData.Application.Interfaces.MarketInstrumentInfo;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;
using Pi.SetMarketData.Application.Services.MarketData.MarketInstrumentInfo;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketInstrumentInfo
{
    public class MarketInstrumentInfoRequestHandler : GetMarketInstrumentInfoAbstractHandler
    {
        private readonly IEntityCacheService _entityCacheService;

        /// <summary>
        ///
        /// </summary>
        /// <param name="entityCacheService"></param>
        public MarketInstrumentInfoRequestHandler(
            IEntityCacheService entityCacheService
        )
        {
            _entityCacheService = entityCacheService;
        }

        protected override async Task<PostMarketInstrumentInfoResponse> Handle(
            PostMarketInstrumentInfoRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var instruments = await _entityCacheService.GetInstrumentBySymbol(request.Data.Symbol);

                var orderBookId = request.Data.Venue == "Equity"
                    ? instruments?.OrderBookId
                    : instruments?.UnderlyingOrderBookID;

                // Get trading sign
                var tradingSign = new Domain.Entities.TradingSign();
                if (orderBookId.HasValue)
                    tradingSign = await _entityCacheService.GetTradingSignByOrderBookId(orderBookId.Value)
                        ?? new Domain.Entities.TradingSign();

                var result = MarketInstrumentInfoService.GetResult(tradingSign);
                return new PostMarketInstrumentInfoResponse(result);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}

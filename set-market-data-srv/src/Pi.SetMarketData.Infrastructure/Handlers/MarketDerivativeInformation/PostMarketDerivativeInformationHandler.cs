using Pi.SetMarketData.Application.Interfaces.MarketDerivativeInformation;
using Pi.SetMarketData.Application.Queries.MarketDerivativeInformation;
using Pi.SetMarketData.Application.Services.MarketData.MarketDerivativeInformation;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketDerivativeInformation;

public class PostMarketDerivativeInformationHandler : PostMarketDerivativeInformationAbstractHandler
{
    private readonly IEntityCacheService _entityCacheService;
    private readonly IMongoService<InitialMargin> _initialMarginService;

    public PostMarketDerivativeInformationHandler(
        IEntityCacheService entityCacheService,
        IMongoService<InitialMargin> initialMarginService
    )
    {
        _entityCacheService = entityCacheService;
        _initialMarginService = initialMarginService;
    }

    protected override async Task<PostMarketDerivativeInformationResponse> Handle(
        PostMarketDerivativeInformationRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var derivativeInstruments =
                await _entityCacheService.GetInstrumentBySymbol(request.Data.Symbol ?? "") ?? new Domain.Entities.Instrument();

            var instrumentDetail =
                await _entityCacheService.GetInstrumentDetailByOrderBookId(
                    derivativeInstruments.OrderBookId
                ) ?? new Domain.Entities.InstrumentDetail();

            // Use underlying order book ID to get the Main symbol from instrument table
            var instruments =
                await _entityCacheService.GetInstrumentByOrderBookId(derivativeInstruments.UnderlyingOrderBookID ?? 0
                ) ?? new Domain.Entities.Instrument();

            // Get initial margin data from by using symbol from main instrument table
            var initialMargin =
                await _initialMarginService.GetByFilterAsync(x => x.Symbol == instruments.Symbol)
                ?? new InitialMargin();

            var result = MarketDerivativeInformationService.GetResult(
                derivativeInstruments,
                instrumentDetail,
                initialMargin,
                request.InitialMarginMultiplier
            );

            return new PostMarketDerivativeInformationResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

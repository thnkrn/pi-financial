using Pi.GlobalMarketData.Application.Interfaces.MarketDerivativeInformation;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Services.MarketData.MarketDerivativeInformation;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.MarketDerivativeInformation
{
    public class PostMarketDerivativeInformationHandler
        : PostMarketDerivativeInformationAbstractHandler
    {
        private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

        public PostMarketDerivativeInformationHandler(
            IMongoService<Domain.Entities.Instrument> instrumentService
        )
        {
            _instrumentService = instrumentService;
        }

        protected override async Task<PostMarketDerivativeInformationResponse> Handle(
            PostMarketDerivativeInformationRequest request,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var instruments = await _instrumentService.GetByFilterAsync(x =>
                    x.Symbol == request.Data.Symbol
                );

                var result = MarketDerivativeInformationService.GetResult(instruments);

                return new PostMarketDerivativeInformationResponse(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

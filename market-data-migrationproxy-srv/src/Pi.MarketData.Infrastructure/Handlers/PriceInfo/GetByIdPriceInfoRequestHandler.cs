using Pi.MarketData.Application.Interfaces.PriceInfo;
using Pi.MarketData.Application.Queries.PriceInfo;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PriceInfo;

public class GetByIdPriceInfoRequestHandler : GetByIdPriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    public GetByIdPriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> priceInfoService)
    {
        _priceInfoService = priceInfoService;
    }

    protected override async Task<GetByIdPriceInfoResponse> Handle(GetByIdPriceInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _priceInfoService.GetByIdAsync(request.id);
            return new GetByIdPriceInfoResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
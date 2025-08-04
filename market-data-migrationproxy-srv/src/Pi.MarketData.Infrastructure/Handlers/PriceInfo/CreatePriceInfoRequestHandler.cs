using Pi.MarketData.Application.Commands.PriceInfo;
using Pi.MarketData.Application.Interfaces.PriceInfo;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PriceInfo;

public class CreatePriceInfoRequestHandler : CreatePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    public CreatePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> priceInfoService)
    {
        _priceInfoService = priceInfoService;
    }

    protected override async Task<CreatePriceInfoResponse> Handle(CreatePriceInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _priceInfoService.CreateAsync(request.PriceInfo);
            return new CreatePriceInfoResponse(true, request.PriceInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
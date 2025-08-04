using Pi.SetMarketData.Application.Commands.PriceInfo;
using Pi.SetMarketData.Application.Interfaces.PriceInfo;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PriceInfo;

public class CreatePriceInfoRequestHandler : CreatePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    public CreatePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> priceInfoService)
    {
        _priceInfoService = priceInfoService;
    }

    protected override async Task<CreatePriceInfoResponse> Handle(CreatePriceInfoRequest request, CancellationToken cancellationToken)
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
using Pi.MarketData.Application.Commands.PriceInfo;
using Pi.MarketData.Application.Interfaces.PriceInfo;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PriceInfo;

public class DeletePriceInfoRequestHandler : DeletePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    public DeletePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> priceInfoService)
    {
        _priceInfoService = priceInfoService;
    }

    protected override async Task<DeletePriceInfoResponse> Handle(DeletePriceInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _priceInfoService.DeleteAsync(request.id);
            return new DeletePriceInfoResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.SetMarketData.Application.Commands.PriceInfo;
using Pi.SetMarketData.Application.Interfaces.PriceInfo;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PriceInfo;

public class DeletePriceInfoRequestHandler : DeletePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _priceInfoService;

    public DeletePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> priceInfoService)
    {
        _priceInfoService = priceInfoService;
    }

    protected override async Task<DeletePriceInfoResponse> Handle(DeletePriceInfoRequest request, CancellationToken cancellationToken)
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
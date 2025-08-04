using Pi.MarketData.Application.Commands.PriceInfo;
using Pi.MarketData.Application.Interfaces.PriceInfo;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.PriceInfo;

public class UpdatePriceInfoRequestHandler : UpdatePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _PriceInfoService;

    public UpdatePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> PriceInfoService)
    {
        _PriceInfoService = PriceInfoService;
    }

    protected override async Task<UpdatePriceInfoResponse> Handle(UpdatePriceInfoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _PriceInfoService.UpdateAsync(request.id, request.PriceInfo);
            return new UpdatePriceInfoResponse(true, request.PriceInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
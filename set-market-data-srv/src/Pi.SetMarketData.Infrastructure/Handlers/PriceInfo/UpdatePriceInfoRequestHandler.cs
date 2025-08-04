using Pi.SetMarketData.Application.Commands.PriceInfo;
using Pi.SetMarketData.Application.Interfaces.PriceInfo;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PriceInfo;

public class UpdatePriceInfoRequestHandler : UpdatePriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _PriceInfoService;

    public UpdatePriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> PriceInfoService)
    {
        _PriceInfoService = PriceInfoService;
    }

    protected override async Task<UpdatePriceInfoResponse> Handle(UpdatePriceInfoRequest request, CancellationToken cancellationToken)
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
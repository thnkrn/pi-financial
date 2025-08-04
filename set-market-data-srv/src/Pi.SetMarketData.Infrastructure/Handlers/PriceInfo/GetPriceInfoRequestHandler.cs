using Pi.SetMarketData.Application.Interfaces.PriceInfo;
using Pi.SetMarketData.Application.Queries.PriceInfo;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.PriceInfo;

public class GetPriceInfoRequestHandler: GetPriceInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.PriceInfo> _PriceInfoService;
    
    public GetPriceInfoRequestHandler(IMongoService<Domain.Entities.PriceInfo> PriceInfoService)
    {
        _PriceInfoService = PriceInfoService;
    }
    
    protected override async Task<GetPriceInfoResponse> Handle(GetPriceInfoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _PriceInfoService.GetAllAsync();
            return new GetPriceInfoResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
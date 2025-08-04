using Pi.SetMarketData.Application.Interfaces.Indicator;
using Pi.SetMarketData.Application.Queries.Indicator;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Indicator;

public class GetIndicatorRequestHandler: GetIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Indicator> _IndicatorService;
    
    public GetIndicatorRequestHandler(IMongoService<Domain.Entities.Indicator> IndicatorService)
    {
        _IndicatorService = IndicatorService;
    }
    
    protected override async Task<GetIndicatorResponse> Handle(GetIndicatorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _IndicatorService.GetAllAsync();
            return new GetIndicatorResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
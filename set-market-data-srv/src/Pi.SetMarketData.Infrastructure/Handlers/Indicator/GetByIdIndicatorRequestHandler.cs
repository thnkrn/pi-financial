using Pi.SetMarketData.Application.Queries.Indicator;
using Pi.SetMarketData.Application.Interfaces.Indicator;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Indicator;

public class GetByIdIndicatorRequestHandler : GetByIdIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Indicator> _indicatorService;

    public GetByIdIndicatorRequestHandler(IMongoService<Domain.Entities.Indicator> indicatorService)
    {
        _indicatorService = indicatorService;
    }

    protected override async Task<GetByIdIndicatorResponse> Handle(GetByIdIndicatorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _indicatorService.GetByIdAsync(request.id);
            return new GetByIdIndicatorResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
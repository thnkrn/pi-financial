using Pi.MarketData.Application.Interfaces.MarketStatus;
using Pi.MarketData.Application.Queries.MarketStatus;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketStatus;

public class GetByIdMarketStatusRequestHandler : GetByIdMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _marketStatusService;

    public GetByIdMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> marketStatusService)
    {
        _marketStatusService = marketStatusService;
    }

    protected override async Task<GetByIdMarketStatusResponse> Handle(GetByIdMarketStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _marketStatusService.GetByIdAsync(request.id);
            return new GetByIdMarketStatusResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
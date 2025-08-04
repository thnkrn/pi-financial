using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Application.Interfaces.MarketStatus;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketStatus;

public class GetByIdMarketStatusRequestHandler : GetByIdMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _marketStatusService;

    public GetByIdMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> marketStatusService)
    {
        _marketStatusService = marketStatusService;
    }

    protected override async Task<GetByIdMarketStatusResponse> Handle(GetByIdMarketStatusRequest request, CancellationToken cancellationToken)
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
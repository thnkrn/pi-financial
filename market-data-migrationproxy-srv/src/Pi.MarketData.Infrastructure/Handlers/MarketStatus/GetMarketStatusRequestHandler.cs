using Pi.MarketData.Application.Interfaces.MarketStatus;
using Pi.MarketData.Application.Queries.MarketStatus;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketStatus;

public class GetMarketStatusRequestHandler : GetMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _MarketStatusService;

    public GetMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> MarketStatusService)
    {
        _MarketStatusService = MarketStatusService;
    }

    protected override async Task<GetMarketStatusResponse> Handle(GetMarketStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _MarketStatusService.GetAllAsync();
            return new GetMarketStatusResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.MarketData.Application.Commands.MarketStatus;
using Pi.MarketData.Application.Interfaces.MarketStatus;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketStatus;

public class CreateMarketStatusRequestHandler : CreateMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _marketStatusService;

    public CreateMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> marketStatusService)
    {
        _marketStatusService = marketStatusService;
    }

    protected override async Task<CreateMarketStatusResponse> Handle(CreateMarketStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _marketStatusService.CreateAsync(request.MarketStatus);
            return new CreateMarketStatusResponse(true, request.MarketStatus);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
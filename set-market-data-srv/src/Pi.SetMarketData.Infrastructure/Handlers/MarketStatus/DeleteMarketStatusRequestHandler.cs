using Pi.SetMarketData.Application.Commands.MarketStatus;
using Pi.SetMarketData.Application.Interfaces.MarketStatus;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketStatus;

public class DeleteMarketStatusRequestHandler : DeleteMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _marketStatusService;

    public DeleteMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> marketStatusService)
    {
        _marketStatusService = marketStatusService;
    }

    protected override async Task<DeleteMarketStatusResponse> Handle(DeleteMarketStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _marketStatusService.DeleteAsync(request.id);
            return new DeleteMarketStatusResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
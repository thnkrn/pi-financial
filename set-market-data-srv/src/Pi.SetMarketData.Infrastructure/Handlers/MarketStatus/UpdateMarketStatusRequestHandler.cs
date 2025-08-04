using Pi.SetMarketData.Application.Commands.MarketStatus;
using Pi.SetMarketData.Application.Interfaces.MarketStatus;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.MarketStatus;

public class UpdateMarketStatusRequestHandler : UpdateMarketStatusRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.MarketStatus> _MarketStatusService;

    public UpdateMarketStatusRequestHandler(IMongoService<Domain.Entities.MarketStatus> MarketStatusService)
    {
        _MarketStatusService = MarketStatusService;
    }

    protected override async Task<UpdateMarketStatusResponse> Handle(UpdateMarketStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _MarketStatusService.UpdateAsync(request.id, request.MarketStatus);
            return new UpdateMarketStatusResponse(true, request.MarketStatus);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
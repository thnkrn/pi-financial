using Pi.MarketData.Application.Commands.Indicator;
using Pi.MarketData.Application.Interfaces.Indicator;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Indicator;

public class DeleteIndicatorRequestHandler : DeleteIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Indicator> _indicatorService;

    public DeleteIndicatorRequestHandler(IMongoService<Domain.Entities.Indicator> indicatorService)
    {
        _indicatorService = indicatorService;
    }

    protected override async Task<DeleteIndicatorResponse> Handle(DeleteIndicatorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _indicatorService.DeleteAsync(request.id);
            return new DeleteIndicatorResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
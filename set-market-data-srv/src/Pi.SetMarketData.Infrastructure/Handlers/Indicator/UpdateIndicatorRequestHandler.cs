using Pi.SetMarketData.Application.Commands.Indicator;
using Pi.SetMarketData.Application.Interfaces.Indicator;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Indicator;

public class UpdateIndicatorRequestHandler : UpdateIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Indicator> _IndicatorService;

    public UpdateIndicatorRequestHandler(IMongoService<Domain.Entities.Indicator> IndicatorService)
    {
        _IndicatorService = IndicatorService;
    }

    protected override async Task<UpdateIndicatorResponse> Handle(UpdateIndicatorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _IndicatorService.UpdateAsync(request.id, request.Indicator);
            return new UpdateIndicatorResponse(true, request.Indicator);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
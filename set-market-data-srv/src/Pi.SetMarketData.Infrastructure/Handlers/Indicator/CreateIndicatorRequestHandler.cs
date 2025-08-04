using Pi.SetMarketData.Application.Commands.Indicator;
using Pi.SetMarketData.Application.Interfaces.Indicator;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Indicator;

public class CreateIndicatorRequestHandler : CreateIndicatorRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Indicator> _indicatorService;

    public CreateIndicatorRequestHandler(IMongoService<Domain.Entities.Indicator> indicatorService)
    {
        _indicatorService = indicatorService;
    }

    protected override async Task<CreateIndicatorResponse> Handle(CreateIndicatorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _indicatorService.CreateAsync(request.Indicator);
            return new CreateIndicatorResponse(true, request.Indicator);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
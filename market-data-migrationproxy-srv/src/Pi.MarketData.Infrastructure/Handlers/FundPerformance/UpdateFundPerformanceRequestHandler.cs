using Pi.MarketData.Application.Commands.FundPerformance;
using Pi.MarketData.Application.Interfaces.FundPerformance;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundPerformance;

public class UpdateFundPerformanceRequestHandler : UpdateFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _FundPerformanceService;

    public UpdateFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> FundPerformanceService)
    {
        _FundPerformanceService = FundPerformanceService;
    }

    protected override async Task<UpdateFundPerformanceResponse> Handle(UpdateFundPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _FundPerformanceService.UpdateAsync(request.id, request.FundPerformance);
            return new UpdateFundPerformanceResponse(true, request.FundPerformance);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
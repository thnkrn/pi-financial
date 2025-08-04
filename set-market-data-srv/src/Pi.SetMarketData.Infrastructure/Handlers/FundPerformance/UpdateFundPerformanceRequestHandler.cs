using Pi.SetMarketData.Application.Commands.FundPerformance;
using Pi.SetMarketData.Application.Interfaces.FundPerformance;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundPerformance;

public class UpdateFundPerformanceRequestHandler : UpdateFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _FundPerformanceService;

    public UpdateFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> FundPerformanceService)
    {
        _FundPerformanceService = FundPerformanceService;
    }

    protected override async Task<UpdateFundPerformanceResponse> Handle(UpdateFundPerformanceRequest request, CancellationToken cancellationToken)
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
using Pi.MarketData.Application.Commands.FundPerformance;
using Pi.MarketData.Application.Interfaces.FundPerformance;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundPerformance;

public class DeleteFundPerformanceRequestHandler : DeleteFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _fundPerformanceService;

    public DeleteFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> fundPerformanceService)
    {
        _fundPerformanceService = fundPerformanceService;
    }

    protected override async Task<DeleteFundPerformanceResponse> Handle(DeleteFundPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _fundPerformanceService.DeleteAsync(request.id);
            return new DeleteFundPerformanceResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
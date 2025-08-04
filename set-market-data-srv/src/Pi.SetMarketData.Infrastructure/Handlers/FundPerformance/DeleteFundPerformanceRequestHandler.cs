using Pi.SetMarketData.Application.Commands.FundPerformance;
using Pi.SetMarketData.Application.Interfaces.FundPerformance;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundPerformance;

public class DeleteFundPerformanceRequestHandler : DeleteFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _fundPerformanceService;

    public DeleteFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> fundPerformanceService)
    {
        _fundPerformanceService = fundPerformanceService;
    }

    protected override async Task<DeleteFundPerformanceResponse> Handle(DeleteFundPerformanceRequest request, CancellationToken cancellationToken)
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
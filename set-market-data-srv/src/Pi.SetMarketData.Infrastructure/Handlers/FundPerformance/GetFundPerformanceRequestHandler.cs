using Pi.SetMarketData.Application.Interfaces.FundPerformance;
using Pi.SetMarketData.Application.Queries.FundPerformance;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundPerformance;

public class GetFundPerformanceRequestHandler: GetFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _FundPerformanceService;
    
    public GetFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> FundPerformanceService)
    {
        _FundPerformanceService = FundPerformanceService;
    }
    
    protected override async Task<GetFundPerformanceResponse> Handle(GetFundPerformanceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _FundPerformanceService.GetAllAsync();
            return new GetFundPerformanceResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
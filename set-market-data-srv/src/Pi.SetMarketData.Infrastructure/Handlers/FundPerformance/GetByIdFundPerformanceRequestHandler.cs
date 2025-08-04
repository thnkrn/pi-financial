using Pi.SetMarketData.Application.Queries.FundPerformance;
using Pi.SetMarketData.Application.Interfaces.FundPerformance;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundPerformance;

public class GetByIdFundPerformanceRequestHandler : GetByIdFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _fundPerformanceService;

    public GetByIdFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> fundPerformanceService)
    {
        _fundPerformanceService = fundPerformanceService;
    }

    protected override async Task<GetByIdFundPerformanceResponse> Handle(GetByIdFundPerformanceRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _fundPerformanceService.GetByIdAsync(request.id);
            return new GetByIdFundPerformanceResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
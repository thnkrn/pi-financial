using Pi.MarketData.Application.Commands.FundPerformance;
using Pi.MarketData.Application.Interfaces.FundPerformance;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundPerformance;

public class CreateFundPerformanceRequestHandler : CreateFundPerformanceRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundPerformance> _fundPerformanceService;

    public CreateFundPerformanceRequestHandler(IMongoService<Domain.Entities.FundPerformance> fundPerformanceService)
    {
        _fundPerformanceService = fundPerformanceService;
    }

    protected override async Task<CreateFundPerformanceResponse> Handle(CreateFundPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _fundPerformanceService.CreateAsync(request.FundPerformance);
            return new CreateFundPerformanceResponse(true, request.FundPerformance);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
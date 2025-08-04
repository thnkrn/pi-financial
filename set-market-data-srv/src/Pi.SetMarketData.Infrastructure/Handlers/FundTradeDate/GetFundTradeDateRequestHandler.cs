using Pi.SetMarketData.Application.Interfaces.FundTradeDate;
using Pi.SetMarketData.Application.Queries.FundTradeDate;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundTradeDate;

public class GetFundTradeDateRequestHandler: GetFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _FundTradeDateService;
    
    public GetFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> FundTradeDateService)
    {
        _FundTradeDateService = FundTradeDateService;
    }
    
    protected override async Task<GetFundTradeDateResponse> Handle(GetFundTradeDateRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _FundTradeDateService.GetAllAsync();
            return new GetFundTradeDateResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
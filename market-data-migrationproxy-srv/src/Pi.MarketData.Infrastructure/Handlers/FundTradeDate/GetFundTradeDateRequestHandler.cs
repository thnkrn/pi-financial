using Pi.MarketData.Application.Interfaces.FundTradeDate;
using Pi.MarketData.Application.Queries.FundTradeDate;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundTradeDate;

public class GetFundTradeDateRequestHandler : GetFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _FundTradeDateService;

    public GetFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> FundTradeDateService)
    {
        _FundTradeDateService = FundTradeDateService;
    }

    protected override async Task<GetFundTradeDateResponse> Handle(GetFundTradeDateRequest request,
        CancellationToken cancellationToken)
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
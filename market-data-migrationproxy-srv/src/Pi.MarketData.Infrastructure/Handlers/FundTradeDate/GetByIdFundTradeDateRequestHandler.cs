using Pi.MarketData.Application.Interfaces.FundTradeDate;
using Pi.MarketData.Application.Queries.FundTradeDate;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundTradeDate;

public class GetByIdFundTradeDateRequestHandler : GetByIdFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _fundTradeDateService;

    public GetByIdFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> fundTradeDateService)
    {
        _fundTradeDateService = fundTradeDateService;
    }

    protected override async Task<GetByIdFundTradeDateResponse> Handle(GetByIdFundTradeDateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _fundTradeDateService.GetByIdAsync(request.id);
            return new GetByIdFundTradeDateResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
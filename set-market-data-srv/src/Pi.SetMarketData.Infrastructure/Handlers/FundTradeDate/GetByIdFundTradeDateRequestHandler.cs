using Pi.SetMarketData.Application.Queries.FundTradeDate;
using Pi.SetMarketData.Application.Interfaces.FundTradeDate;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.FundTradeDate;

public class GetByIdFundTradeDateRequestHandler : GetByIdFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _fundTradeDateService;

    public GetByIdFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> fundTradeDateService)
    {
        _fundTradeDateService = fundTradeDateService;
    }

    protected override async Task<GetByIdFundTradeDateResponse> Handle(GetByIdFundTradeDateRequest request, CancellationToken cancellationToken)
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
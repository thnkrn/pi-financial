using Pi.MarketData.Application.Commands.FundTradeDate;
using Pi.MarketData.Application.Interfaces.FundTradeDate;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundTradeDate;

public class DeleteFundTradeDateRequestHandler : DeleteFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _fundTradeDateService;

    public DeleteFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> fundTradeDateService)
    {
        _fundTradeDateService = fundTradeDateService;
    }

    protected override async Task<DeleteFundTradeDateResponse> Handle(DeleteFundTradeDateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _fundTradeDateService.DeleteAsync(request.id);
            return new DeleteFundTradeDateResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
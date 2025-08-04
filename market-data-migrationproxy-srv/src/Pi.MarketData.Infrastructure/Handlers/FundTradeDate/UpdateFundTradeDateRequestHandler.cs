using Pi.MarketData.Application.Commands.FundTradeDate;
using Pi.MarketData.Application.Interfaces.FundTradeDate;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundTradeDate;

public class UpdateFundTradeDateRequestHandler : UpdateFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _FundTradeDateService;

    public UpdateFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> FundTradeDateService)
    {
        _FundTradeDateService = FundTradeDateService;
    }

    protected override async Task<UpdateFundTradeDateResponse> Handle(UpdateFundTradeDateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _FundTradeDateService.UpdateAsync(request.id, request.FundTradeDate);
            return new UpdateFundTradeDateResponse(true, request.FundTradeDate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
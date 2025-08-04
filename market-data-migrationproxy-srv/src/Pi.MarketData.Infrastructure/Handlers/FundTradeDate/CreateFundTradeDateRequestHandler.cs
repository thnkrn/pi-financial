using Pi.MarketData.Application.Commands.FundTradeDate;
using Pi.MarketData.Application.Interfaces.FundTradeDate;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.FundTradeDate;

public class CreateFundTradeDateRequestHandler : CreateFundTradeDateRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.FundTradeDate> _fundTradeDateService;

    public CreateFundTradeDateRequestHandler(IMongoService<Domain.Entities.FundTradeDate> fundTradeDateService)
    {
        _fundTradeDateService = fundTradeDateService;
    }

    protected override async Task<CreateFundTradeDateResponse> Handle(CreateFundTradeDateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _fundTradeDateService.CreateAsync(request.FundTradeDate);
            return new CreateFundTradeDateResponse(true, request.FundTradeDate);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
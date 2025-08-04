using Pi.MarketData.Application.Interfaces.Financial;
using Pi.MarketData.Application.Queries.Financial;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Financial;

public class GetFinancialRequestHandler : GetFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _FinancialService;

    public GetFinancialRequestHandler(IMongoService<Domain.Entities.Financial> FinancialService)
    {
        _FinancialService = FinancialService;
    }

    protected override async Task<GetFinancialResponse> Handle(GetFinancialRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _FinancialService.GetAllAsync();
            return new GetFinancialResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
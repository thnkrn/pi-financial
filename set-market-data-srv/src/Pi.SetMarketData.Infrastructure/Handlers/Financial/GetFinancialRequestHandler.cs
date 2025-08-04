using Pi.SetMarketData.Application.Interfaces.Financial;
using Pi.SetMarketData.Application.Queries.Financial;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Financial;

public class GetFinancialRequestHandler: GetFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _FinancialService;
    
    public GetFinancialRequestHandler(IMongoService<Domain.Entities.Financial> FinancialService)
    {
        _FinancialService = FinancialService;
    }
    
    protected override async Task<GetFinancialResponse> Handle(GetFinancialRequest request, CancellationToken cancellationToken)
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
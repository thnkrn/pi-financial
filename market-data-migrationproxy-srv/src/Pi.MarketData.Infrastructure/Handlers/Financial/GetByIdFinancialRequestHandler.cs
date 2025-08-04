using Pi.MarketData.Application.Interfaces.Financial;
using Pi.MarketData.Application.Queries.Financial;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Financial;

public class GetByIdFinancialRequestHandler : GetByIdFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _financialService;

    public GetByIdFinancialRequestHandler(IMongoService<Domain.Entities.Financial> financialService)
    {
        _financialService = financialService;
    }

    protected override async Task<GetByIdFinancialResponse> Handle(GetByIdFinancialRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _financialService.GetByIdAsync(request.id);
            return new GetByIdFinancialResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
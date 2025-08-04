using Pi.SetMarketData.Application.Queries.Financial;
using Pi.SetMarketData.Application.Interfaces.Financial;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Financial;

public class GetByIdFinancialRequestHandler : GetByIdFinancialRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Financial> _financialService;

    public GetByIdFinancialRequestHandler(IMongoService<Domain.Entities.Financial> financialService)
    {
        _financialService = financialService;
    }

    protected override async Task<GetByIdFinancialResponse> Handle(GetByIdFinancialRequest request, CancellationToken cancellationToken)
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
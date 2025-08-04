using Pi.MarketData.Application.Interfaces.CorporateAction;
using Pi.MarketData.Application.Queries.CorporateAction;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.CorporateAction;

public class GetCorporateActionRequestHandler : GetCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _CorporateActionService;

    public GetCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> CorporateActionService)
    {
        _CorporateActionService = CorporateActionService;
    }

    protected override async Task<GetCorporateActionResponse> Handle(GetCorporateActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _CorporateActionService.GetAllAsync();
            return new GetCorporateActionResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
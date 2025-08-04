using Pi.MarketData.Application.Interfaces.CorporateAction;
using Pi.MarketData.Application.Queries.CorporateAction;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.CorporateAction;

public class GetByIdCorporateActionRequestHandler : GetByIdCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;

    public GetByIdCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> corporateActionService)
    {
        _corporateActionService = corporateActionService;
    }

    protected override async Task<GetByIdCorporateActionResponse> Handle(GetByIdCorporateActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _corporateActionService.GetByIdAsync(request.id);
            return new GetByIdCorporateActionResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
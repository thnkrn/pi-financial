using Pi.MarketData.Application.Commands.CorporateAction;
using Pi.MarketData.Application.Interfaces.CorporateAction;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.CorporateAction;

public class CreateCorporateActionRequestHandler : CreateCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;

    public CreateCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> corporateActionService)
    {
        _corporateActionService = corporateActionService;
    }

    protected override async Task<CreateCorporateActionResponse> Handle(CreateCorporateActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _corporateActionService.CreateAsync(request.CorporateAction);
            return new CreateCorporateActionResponse(true, request.CorporateAction);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
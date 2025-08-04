using Pi.MarketData.Application.Commands.CorporateAction;
using Pi.MarketData.Application.Interfaces.CorporateAction;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.CorporateAction;

public class DeleteCorporateActionRequestHandler : DeleteCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;

    public DeleteCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> corporateActionService)
    {
        _corporateActionService = corporateActionService;
    }

    protected override async Task<DeleteCorporateActionResponse> Handle(DeleteCorporateActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _corporateActionService.DeleteAsync(request.id);
            return new DeleteCorporateActionResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
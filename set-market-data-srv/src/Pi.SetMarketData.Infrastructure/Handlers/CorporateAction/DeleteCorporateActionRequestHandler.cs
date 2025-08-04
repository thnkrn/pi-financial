using Pi.SetMarketData.Application.Commands.CorporateAction;
using Pi.SetMarketData.Application.Interfaces.CorporateAction;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.CorporateAction;

public class DeleteCorporateActionRequestHandler : DeleteCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _corporateActionService;

    public DeleteCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> corporateActionService)
    {
        _corporateActionService = corporateActionService;
    }

    protected override async Task<DeleteCorporateActionResponse> Handle(DeleteCorporateActionRequest request, CancellationToken cancellationToken)
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
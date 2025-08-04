using Pi.MarketData.Application.Commands.CorporateAction;
using Pi.MarketData.Application.Interfaces.CorporateAction;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.CorporateAction;

public class UpdateCorporateActionRequestHandler : UpdateCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _CorporateActionService;

    public UpdateCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> CorporateActionService)
    {
        _CorporateActionService = CorporateActionService;
    }

    protected override async Task<UpdateCorporateActionResponse> Handle(UpdateCorporateActionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _CorporateActionService.UpdateAsync(request.id, request.CorporateAction);
            return new UpdateCorporateActionResponse(true, request.CorporateAction);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
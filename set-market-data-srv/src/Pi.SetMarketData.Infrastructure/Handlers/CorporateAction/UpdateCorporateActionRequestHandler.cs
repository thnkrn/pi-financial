using Pi.SetMarketData.Application.Commands.CorporateAction;
using Pi.SetMarketData.Application.Interfaces.CorporateAction;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.CorporateAction;

public class UpdateCorporateActionRequestHandler : UpdateCorporateActionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.CorporateAction> _CorporateActionService;

    public UpdateCorporateActionRequestHandler(IMongoService<Domain.Entities.CorporateAction> CorporateActionService)
    {
        _CorporateActionService = CorporateActionService;
    }

    protected override async Task<UpdateCorporateActionResponse> Handle(UpdateCorporateActionRequest request, CancellationToken cancellationToken)
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
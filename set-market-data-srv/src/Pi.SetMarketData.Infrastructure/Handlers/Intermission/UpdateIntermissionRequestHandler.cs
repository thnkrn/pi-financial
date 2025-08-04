using Pi.SetMarketData.Application.Commands.Intermission;
using Pi.SetMarketData.Application.Interfaces.Intermission;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Intermission;

public class UpdateIntermissionRequestHandler : UpdateIntermissionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Intermission> _IntermissionService;

    public UpdateIntermissionRequestHandler(IMongoService<Domain.Entities.Intermission> IntermissionService)
    {
        _IntermissionService = IntermissionService;
    }

    protected override async Task<UpdateIntermissionResponse> Handle(UpdateIntermissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _IntermissionService.UpdateAsync(request.id, request.Intermission);
            return new UpdateIntermissionResponse(true, request.Intermission);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
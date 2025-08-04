using Pi.MarketData.Application.Commands.Intermission;
using Pi.MarketData.Application.Interfaces.Intermission;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Intermission;

public class DeleteIntermissionRequestHandler : DeleteIntermissionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Intermission> _intermissionService;

    public DeleteIntermissionRequestHandler(IMongoService<Domain.Entities.Intermission> intermissionService)
    {
        _intermissionService = intermissionService;
    }

    protected override async Task<DeleteIntermissionResponse> Handle(DeleteIntermissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _intermissionService.DeleteAsync(request.id);
            return new DeleteIntermissionResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
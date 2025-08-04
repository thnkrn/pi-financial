using Pi.MarketData.Application.Commands.Intermission;
using Pi.MarketData.Application.Interfaces.Intermission;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Intermission;

public class CreateIntermissionRequestHandler : CreateIntermissionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Intermission> _intermissionService;

    public CreateIntermissionRequestHandler(IMongoService<Domain.Entities.Intermission> intermissionService)
    {
        _intermissionService = intermissionService;
    }

    protected override async Task<CreateIntermissionResponse> Handle(CreateIntermissionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _intermissionService.CreateAsync(request.Intermission);
            return new CreateIntermissionResponse(true, request.Intermission);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
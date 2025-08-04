using Pi.SetMarketData.Application.Queries.Intermission;
using Pi.SetMarketData.Application.Interfaces.Intermission;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Intermission;

public class GetByIdIntermissionRequestHandler : GetByIdIntermissionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Intermission> _intermissionService;

    public GetByIdIntermissionRequestHandler(IMongoService<Domain.Entities.Intermission> intermissionService)
    {
        _intermissionService = intermissionService;
    }

    protected override async Task<GetByIdIntermissionResponse> Handle(GetByIdIntermissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _intermissionService.GetByIdAsync(request.id);
            return new GetByIdIntermissionResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.SetMarketData.Application.Interfaces.Intermission;
using Pi.SetMarketData.Application.Queries.Intermission;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Intermission;

public class GetIntermissionRequestHandler: GetIntermissionRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Intermission> _IntermissionService;
    
    public GetIntermissionRequestHandler(IMongoService<Domain.Entities.Intermission> IntermissionService)
    {
        _IntermissionService = IntermissionService;
    }
    
    protected override async Task<GetIntermissionResponse> Handle(GetIntermissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _IntermissionService.GetAllAsync();
            return new GetIntermissionResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
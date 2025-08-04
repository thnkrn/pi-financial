using Pi.MarketData.Application.Interfaces.Filter;
using Pi.MarketData.Application.Queries.Filter;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Filter;

public class GetFilterRequestHandler : GetFilterRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Filter> _FilterService;

    public GetFilterRequestHandler(IMongoService<Domain.Entities.Filter> FilterService)
    {
        _FilterService = FilterService;
    }

    protected override async Task<GetFilterResponse> Handle(GetFilterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _FilterService.GetAllAsync();
            return new GetFilterResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
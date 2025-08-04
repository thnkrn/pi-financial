using Pi.MarketData.Application.Interfaces.Filter;
using Pi.MarketData.Application.Queries.Filter;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Filter;

public class GetByIdFilterRequestHandler : GetByIdFilterRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Filter> _filterService;

    public GetByIdFilterRequestHandler(IMongoService<Domain.Entities.Filter> filterService)
    {
        _filterService = filterService;
    }

    protected override async Task<GetByIdFilterResponse> Handle(GetByIdFilterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _filterService.GetByIdAsync(request.id);
            return new GetByIdFilterResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
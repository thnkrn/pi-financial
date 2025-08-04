using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Domain.Entities;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class GetByIdCuratedFilterRequestHandler : GetByIdCuratedFilterRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;

    public GetByIdCuratedFilterRequestHandler(IMongoService<CuratedFilter> curatedFilterService)
    {
        _curatedFilterService = curatedFilterService;
    }

    protected override async Task<GetByIdCuratedFilterResponse> Handle(GetByIdCuratedFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _curatedFilterService.GetByIdAsync(request.id);
            return new GetByIdCuratedFilterResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
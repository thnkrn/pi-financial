using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Domain.Entities;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
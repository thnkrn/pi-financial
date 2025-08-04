using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class CreateCuratedFilterRequestHandler : CreateCuratedFilterRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;

    public CreateCuratedFilterRequestHandler(IMongoService<CuratedFilter> curatedFilterService)
    {
        _curatedFilterService = curatedFilterService;
    }

    protected override async Task<CreateCuratedFilterResponse> Handle(CreateCuratedFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _curatedFilterService.UpsertManyAsync(request.CuratedFilter, x => x.FilterId);
            return new CreateCuratedFilterResponse(true, request.CuratedFilter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
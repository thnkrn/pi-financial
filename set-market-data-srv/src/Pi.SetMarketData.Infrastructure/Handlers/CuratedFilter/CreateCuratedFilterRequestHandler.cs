using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
            var result = await _curatedFilterService.UpsertManyAsync(request.CuratedFilter, x => x.FilterId);
            return new CreateCuratedFilterResponse(true, request.CuratedFilter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
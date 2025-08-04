using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class CreateCuratedListRequestHandler : CreateCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public CreateCuratedListRequestHandler(IMongoService<CuratedList> curatedListService)
    {
        _curatedListService = curatedListService;
    }

    protected override async Task<CreateCuratedListResponse> Handle(CreateCuratedListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _curatedListService.UpsertManyAsync(request.CuratedList, x => x.CuratedListId);
            return new CreateCuratedListResponse(true, request.CuratedList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
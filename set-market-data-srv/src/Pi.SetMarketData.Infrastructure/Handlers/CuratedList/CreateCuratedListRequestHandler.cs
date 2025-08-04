using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
            var result = await _curatedListService.UpsertManyAsync(request.CuratedList, x => x.CuratedListId);
            return new CreateCuratedListResponse(true, request.CuratedList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
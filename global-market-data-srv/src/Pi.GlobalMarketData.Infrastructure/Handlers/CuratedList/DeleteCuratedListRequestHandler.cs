using MongoDB.Bson;
using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class DeleteCuratedListRequestHandler : DeleteCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public DeleteCuratedListRequestHandler(IMongoService<CuratedList> curatedListService)
    {
        _curatedListService = curatedListService;
    }

    protected override async Task<DeleteCuratedListResponse> Handle(DeleteCuratedListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (ObjectId.TryParse(request.id, out var objectId))
            {
                var curatedList = await _curatedListService.GetByIdAsync(request.id);
                if (curatedList == null) return new DeleteCuratedListResponse(false, "NotFound");
                await _curatedListService.DeleteAsync(request.id);
                return new DeleteCuratedListResponse(true, null);
            }
            return new DeleteCuratedListResponse(false, "BadRequest");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
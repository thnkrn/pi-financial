using MongoDB.Bson;
using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class DeleteCuratedFilterRequestHandler : DeleteCuratedFilterRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _curatedFilterService;

    public DeleteCuratedFilterRequestHandler(IMongoService<CuratedFilter> curatedFilterService)
    {
        _curatedFilterService = curatedFilterService;
    }

    protected override async Task<DeleteCuratedFilterResponse> Handle(DeleteCuratedFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (ObjectId.TryParse(request.id, out var objectId))
            {
                var curatedList = await _curatedFilterService.GetByIdAsync(request.id);
                if (curatedList == null) return new DeleteCuratedFilterResponse(false, "NotFound");
                await _curatedFilterService.DeleteAsync(request.id);
                return new DeleteCuratedFilterResponse(true, null);
            }
            return new DeleteCuratedFilterResponse(false, "BadRequest");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
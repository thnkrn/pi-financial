using MongoDB.Bson;
using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
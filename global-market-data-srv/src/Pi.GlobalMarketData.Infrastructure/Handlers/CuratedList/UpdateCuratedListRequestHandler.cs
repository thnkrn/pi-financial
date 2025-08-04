using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class UpdateCuratedListRequestHandler : UpdateCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _curatedListService;

    public UpdateCuratedListRequestHandler(IMongoService<CuratedList> CuratedListService)
    {
        _curatedListService = CuratedListService;
    }

    protected override async Task<UpdateCuratedListResponse> Handle(UpdateCuratedListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            request.CuratedList.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await _curatedListService.UpdateAsync(request.id, request.CuratedList);
            return new UpdateCuratedListResponse(true, request.CuratedList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
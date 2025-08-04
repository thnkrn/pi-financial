using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

public class UpdateCuratedListRequestHandler : UpdateCuratedListRequestAbstractHandler
{
    private readonly IMongoService<CuratedList> _CuratedListService;

    public UpdateCuratedListRequestHandler(IMongoService<CuratedList> CuratedListService)
    {
        _CuratedListService = CuratedListService;
    }

    protected override async Task<UpdateCuratedListResponse> Handle(UpdateCuratedListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            request.CuratedList.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await _CuratedListService.UpdateAsync(request.id, request.CuratedList);
            return new UpdateCuratedListResponse(true, request.CuratedList);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
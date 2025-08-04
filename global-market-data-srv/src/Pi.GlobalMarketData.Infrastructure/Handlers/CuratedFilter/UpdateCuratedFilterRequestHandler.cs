using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Interfaces;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers;

public class UpdateCuratedFilterRequestHandler : UpdateCuratedFilterRequestAbstractHandler
{
    private readonly IMongoService<CuratedFilter> _CuratedFilterService;

    public UpdateCuratedFilterRequestHandler(IMongoService<CuratedFilter> CuratedFilterService)
    {
        _CuratedFilterService = CuratedFilterService;
    }

    protected override async Task<UpdateCuratedFilterResponse> Handle(UpdateCuratedFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _CuratedFilterService.UpdateAsync(request.id, request.CuratedFilter);
            return new UpdateCuratedFilterResponse(true, request.CuratedFilter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
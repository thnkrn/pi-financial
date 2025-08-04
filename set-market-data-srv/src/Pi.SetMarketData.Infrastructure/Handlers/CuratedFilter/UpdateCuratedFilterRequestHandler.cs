using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;

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
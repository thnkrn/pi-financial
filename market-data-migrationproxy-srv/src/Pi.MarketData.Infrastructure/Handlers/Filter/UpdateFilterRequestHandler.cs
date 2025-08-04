using Pi.MarketData.Application.Commands.Filter;
using Pi.MarketData.Application.Interfaces.Filter;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Filter;

public class UpdateFilterRequestHandler : UpdateFilterRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Filter> _FilterService;

    public UpdateFilterRequestHandler(IMongoService<Domain.Entities.Filter> FilterService)
    {
        _FilterService = FilterService;
    }

    protected override async Task<UpdateFilterResponse> Handle(UpdateFilterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _FilterService.UpdateAsync(request.id, request.Filter);
            return new UpdateFilterResponse(true, request.Filter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
using Pi.SetMarketData.Application.Commands.Filter;
using Pi.SetMarketData.Application.Interfaces.Filter;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Filter;

public class DeleteFilterRequestHandler : DeleteFilterRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Filter> _filterService;

    public DeleteFilterRequestHandler(IMongoService<Domain.Entities.Filter> filterService)
    {
        _filterService = filterService;
    }

    protected override async Task<DeleteFilterResponse> Handle(DeleteFilterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _filterService.DeleteAsync(request.id);
            return new DeleteFilterResponse(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
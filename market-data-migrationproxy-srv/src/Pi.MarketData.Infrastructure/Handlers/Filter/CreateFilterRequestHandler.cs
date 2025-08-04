using Pi.MarketData.Application.Commands.Filter;
using Pi.MarketData.Application.Interfaces.Filter;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Filter;

public class CreateFilterRequestHandler : CreateFilterRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Filter> _filterService;

    public CreateFilterRequestHandler(IMongoService<Domain.Entities.Filter> filterService)
    {
        _filterService = filterService;
    }

    protected override async Task<CreateFilterResponse> Handle(CreateFilterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _filterService.CreateAsync(request.Filter);
            return new CreateFilterResponse(true, request.Filter);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
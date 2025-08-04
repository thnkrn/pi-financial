using Pi.GlobalMarketData.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.GeInstrument;

public class GetByIdGeInstrumentRequestHandler : GetByIdGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetByIdGeInstrumentRequestHandler(
        IMongoService<Domain.Entities.GeInstrument> geInstrumentService
    )
    {
        _geInstrumentService = geInstrumentService;
    }

    protected override async Task<GetByIdGeInstrumentResponse> Handle(
        GetByIdGeInstrumentRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await _geInstrumentService.GetByIdAsync(request.id);
            return new GetByIdGeInstrumentResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

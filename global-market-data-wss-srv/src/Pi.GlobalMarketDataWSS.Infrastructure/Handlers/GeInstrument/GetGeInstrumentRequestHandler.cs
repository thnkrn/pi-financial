using Pi.GlobalMarketDataWSS.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Handlers.GeInstrument;

public class GetGeInstrumentRequestHandler : GetGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetGeInstrumentRequestHandler(IMongoService<Domain.Entities.GeInstrument> geinstrumentService)
    {
        _geInstrumentService = geinstrumentService;
    }

    protected override async Task<GetGeInstrumentResponse> Handle(GetGeInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _geInstrumentService.GetAllAsync();
            return new GetGeInstrumentResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
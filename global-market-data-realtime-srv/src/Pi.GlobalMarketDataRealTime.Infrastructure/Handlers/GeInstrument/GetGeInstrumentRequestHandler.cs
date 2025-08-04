using Pi.GlobalMarketDataRealTime.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Handlers.GeInstrument;

public class GetGeInstrumentRequestHandler : GetGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    /// <summary>
    /// </summary>
    /// <param name="geinstrumentService"></param>
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
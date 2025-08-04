using Pi.GlobalMarketDataRealTime.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Handlers.GeInstrument;

public class GetByIdGeInstrumentRequestHandler : GetByIdGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    /// <summary>
    /// </summary>
    /// <param name="geInstrumentService"></param>
    public GetByIdGeInstrumentRequestHandler(IMongoService<Domain.Entities.GeInstrument> geInstrumentService)
    {
        _geInstrumentService = geInstrumentService;
    }

    protected override async Task<GetByIdGeInstrumentResponse> Handle(GetByIdGeInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _geInstrumentService.GetByIdAsync(request.Id);
            return new GetByIdGeInstrumentResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
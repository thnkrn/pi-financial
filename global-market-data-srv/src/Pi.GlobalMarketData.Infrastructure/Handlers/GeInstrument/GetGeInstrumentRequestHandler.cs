using Pi.GlobalMarketData.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketData.Infrastructure.Handlers.GeInstrument;

public class GetGeInstrumentRequestHandler : GetGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetGeInstrumentRequestHandler(
        IMongoService<Domain.Entities.GeInstrument> geinstrumentService
    )
    {
        _geInstrumentService = geinstrumentService;
    }

    protected override async Task<GetGeInstrumentResponse> Handle(
        GetGeInstrumentRequest request,
        CancellationToken cancellationToken
    )
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

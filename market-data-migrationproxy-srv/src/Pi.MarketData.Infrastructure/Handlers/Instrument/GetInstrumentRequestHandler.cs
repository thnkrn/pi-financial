using Pi.MarketData.Application.Interfaces.Instrument;
using Pi.MarketData.Application.Queries.Instrument;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.Instrument;

public class GetInstrumentRequestHandler : GetInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public GetInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<GetInstrumentResponse> Handle(GetInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _instrumentService.GetAllAsync();
            return new GetInstrumentResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
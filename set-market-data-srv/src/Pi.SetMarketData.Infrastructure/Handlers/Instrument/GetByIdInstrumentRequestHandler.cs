using Pi.SetMarketData.Application.Queries.Instrument;
using Pi.SetMarketData.Application.Interfaces.Instrument;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers.Instrument;

public class GetByIdInstrumentRequestHandler : GetByIdInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;

    public GetByIdInstrumentRequestHandler(IMongoService<Domain.Entities.Instrument> instrumentService)
    {
        _instrumentService = instrumentService;
    }

    protected override async Task<GetByIdInstrumentResponse> Handle(GetByIdInstrumentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _instrumentService.GetByIdAsync(request.id);
            return new GetByIdInstrumentResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
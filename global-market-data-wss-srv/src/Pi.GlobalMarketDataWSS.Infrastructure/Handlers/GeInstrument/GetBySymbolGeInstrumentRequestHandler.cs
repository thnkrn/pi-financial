using Pi.GlobalMarketDataWSS.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketDataWSS.Application.Queries.GeInstrument;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Handlers.GeInstrument;

public class GetBySymbolGeInstrumentRequestHandler : GetBySymbolGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    public GetBySymbolGeInstrumentRequestHandler(IMongoService<Domain.Entities.GeInstrument> geInstrumentService)
    {
        _geInstrumentService = geInstrumentService;
    }

    protected override async Task<GetBySymbolGeInstrumentResponse> Handle(GetBySymbolGeInstrumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _geInstrumentService.GetAllAsync();

            if (!string.IsNullOrEmpty(request.investmentType))
                result = result.Where(e =>
                    (!string.IsNullOrEmpty(e.Exchange) ? e.Exchange : "").Trim().ToLower()
                    .Contains(request.investmentType.Trim().ToLower()));

            if (!string.IsNullOrEmpty(request.symbol))
                result = result.Where(e =>
                    (!string.IsNullOrEmpty(e.Symbol) ? e.Symbol : "").Trim().ToLower()
                    .Contains(request.symbol.Trim().ToLower()));

            return new GetBySymbolGeInstrumentResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
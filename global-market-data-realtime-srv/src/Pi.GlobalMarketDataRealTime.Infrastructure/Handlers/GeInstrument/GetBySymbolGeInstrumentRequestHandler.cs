using Pi.GlobalMarketDataRealTime.Application.Interfaces.GeInstrument;
using Pi.GlobalMarketDataRealTime.Application.Queries.GeInstrument;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Handlers.GeInstrument;

public class GetBySymbolGeInstrumentRequestHandler : GetBySymbolGeInstrumentRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.GeInstrument> _geInstrumentService;

    /// <summary>
    /// </summary>
    /// <param name="geInstrumentService"></param>
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

            if (!string.IsNullOrEmpty(request.InvestmentType))
                result = result.Where(e =>
                    (!string.IsNullOrEmpty(e.Exchange) ? e.Exchange : "").Trim().ToLower()
                    .Contains(request.InvestmentType.Trim().ToLower()));

            if (!string.IsNullOrEmpty(request.Symbol))
                result = result.Where(e =>
                    (!string.IsNullOrEmpty(e.Symbol) ? e.Symbol : "").Trim().ToLower()
                    .Contains(request.Symbol.Trim().ToLower()));

            return new GetBySymbolGeInstrumentResponse(result.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
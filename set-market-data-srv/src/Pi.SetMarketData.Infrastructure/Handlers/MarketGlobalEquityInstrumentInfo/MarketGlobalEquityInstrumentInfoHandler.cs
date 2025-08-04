using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Application.Interfaces;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Services;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.SetMarketData.Infrastructure.Handlers;
public class MarketGlobalEquityInstrumentInfoHandler : PostMarketGlobalEquityInstrumentInfoRequestAbstractHandler
{
    private readonly IMongoService<Domain.Entities.Instrument> _instrumentService;
    private readonly ILogger<MarketGlobalEquityInstrumentInfoHandler> _logger;
    public MarketGlobalEquityInstrumentInfoHandler(
        IMongoService<Domain.Entities.Instrument> instrumentService,
        ILogger<MarketGlobalEquityInstrumentInfoHandler> logger
    )
    {
        _instrumentService = instrumentService;
        _logger = logger;
    }
    protected override async Task<PostMarketGlobalEquityInstrumentInfoResponse> Handle(
        PostMarketGlobalEquityInstrumentInfoRequest request, 
        CancellationToken cancellationToken)
    {
        var instrument = await _instrumentService.GetByFilterAsync(target =>
            (target.Venue == request.Data.Venue || request.Data.Venue == null)
            && target.Symbol == request.Data.Symbol
        );
        try
        {
            var result = MarketGlobalEquityInstrumentInfoService.GetResult(instrument);
            return new PostMarketGlobalEquityInstrumentInfoResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Handle Http request failed.");
            throw new InvalidOperationException("Failed to get result from service", ex);
        }
    }
}
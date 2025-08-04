using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketTicker;

namespace Pi.SetMarketData.Application.Interfaces.MarketTicker;

public abstract class PostMarketTickerRequestAbstractHandler
    : RequestHandler<PostMarketTickerRequest, PostMarketTickerResponse>;

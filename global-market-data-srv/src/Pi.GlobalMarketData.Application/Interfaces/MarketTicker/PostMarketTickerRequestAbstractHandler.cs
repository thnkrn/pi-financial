using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketTicker;

public abstract class PostMarketTickerRequestAbstractHandler
    : RequestHandler<PostMarketTickerRequest, PostMarketTickerResponse>;

using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Application.Queries.MarketTicker;

namespace Pi.SetMarketData.Application.Interfaces.MarketStatus;

public abstract class PostMarketStatusRequestAbstractHandler
    : RequestHandler<PostMarketStatusRequest, PostMarketStatusResponse>;

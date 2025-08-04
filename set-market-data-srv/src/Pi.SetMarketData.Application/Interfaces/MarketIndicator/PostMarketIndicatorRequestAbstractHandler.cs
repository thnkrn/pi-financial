using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketIndicator;

namespace Pi.SetMarketData.Application.Interfaces.MarketIndicator;

public abstract class PostMarketIndicatorRequestAbstractHandler
    : RequestHandler<PostMarketIndicatorRequest, PostMarketIndicatorResponse>;

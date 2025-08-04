using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketTimelineRendered;

public abstract class PostMarketTimelineRenderedRequestAbstractHandler
    : RequestHandler<PostMarketTimelineRenderedRequest, PostMarketTimelineRenderedResponse>;

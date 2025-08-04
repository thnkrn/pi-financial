using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketTimelineRendered;

namespace Pi.SetMarketData.Application.Interfaces.MarketTimelineRendered;

public abstract class PostMarketTimelineRenderedRequestAbstractHandler
    : RequestHandler<PostMarketTimelineRenderedRequest, PostMarketTimelineRenderedResponse>;

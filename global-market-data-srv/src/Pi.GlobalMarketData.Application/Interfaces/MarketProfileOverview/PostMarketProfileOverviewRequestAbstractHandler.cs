using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketProfileOverview;

public abstract class PostMarketProfileOverviewRequestAbstractHandler
    : RequestHandler<PostMarketProfileOverViewRequest, PostMarketProfileOverviewResponse>;

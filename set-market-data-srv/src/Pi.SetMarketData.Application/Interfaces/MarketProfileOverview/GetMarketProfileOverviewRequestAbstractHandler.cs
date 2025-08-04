using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketProfileOverview;

namespace Pi.SetMarketData.Application.Interfaces.MarketProfileFundamentals;

public abstract class GetMarketProfileOverviewRequestAbstractHandler
    : RequestHandler<PostMarketProfileOverViewRequest, PostMarketProfileOverviewResponse>;

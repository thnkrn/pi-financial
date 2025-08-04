using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries;

namespace Pi.SetMarketData.Application.Interfaces.MarketInitialMargin;

public abstract class GetMarketInitialMarginAbstractHandler
    : RequestHandler<PostMarketInitialMarginRequest, PostMarketInitialMarginResponse>;

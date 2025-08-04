using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketProfileDescription;

namespace Pi.SetMarketData.Application.Interfaces.MarketProfileDescription;

public abstract class GetMarketProfileDescriptionRequestAbstractHandler
    : RequestHandler<PostMarketProfileDescriptionRequest, PostMarketProfileDescriptionResponse>;

using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketProfileDescription;

public abstract class GetMarketProfileDescriptionRequestAbstractHandler
    : RequestHandler<PostMarketProfileDescriptionRequest, PostMarketProfileDescriptionResponse>;

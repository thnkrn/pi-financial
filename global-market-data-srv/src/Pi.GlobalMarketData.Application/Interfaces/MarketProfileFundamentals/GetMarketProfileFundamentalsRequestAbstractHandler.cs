using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketProfileFundamentals;

public abstract class GetMarketProfileFundamentalsRequestAbstractHandler
    : RequestHandler<PostMarketProfileFundamentalsRequest, PostMarketProfileFundamentalsResponse>;

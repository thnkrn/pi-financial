using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;

namespace Pi.SetMarketData.Application.Interfaces.MarketProfileFundamentals;

public abstract class GetMarketProfileFundamentalsRequestAbstractHandler
    : RequestHandler<PostMarketProfileFundamentalsRequest, PostMarketProfileFundamentalsResponse>;

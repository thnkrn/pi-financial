using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;

namespace Pi.SetMarketData.Application.Interfaces.MarketProfileFinancials;

public abstract class GetMarketProfileFinancialsRequestAbstractHandler
    : RequestHandler<PostMarketProfileFinancialsRequest, PostMarketProfileFinancialsResponse>;

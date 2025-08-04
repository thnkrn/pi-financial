using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketInstrumentSearch;

namespace Pi.SetMarketData.Application.Interfaces.MarketInstrumentSearch;

public abstract class PostMarketInstrumentSearchAbstractHandler
    : RequestHandler<PostMarketInstrumentSearchRequest, PostMarketInstrumentSearchResponse>;

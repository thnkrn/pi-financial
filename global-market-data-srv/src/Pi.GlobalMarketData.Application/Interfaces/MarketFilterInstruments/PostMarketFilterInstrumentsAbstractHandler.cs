using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries.MarketFilterInstruments;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketFilterInstruments;

public abstract class PostMarketFilterInstrumentsAbstractHandler
    : RequestHandler<PostMarketFilterInstrumentsRequest, PostMarketFilterInstrumentsResponse>;

using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Queries;

namespace Pi.GlobalMarketData.Application.Interfaces.MarketHomeInstrument;

public abstract class PostHomeInstrumentAbstractHandler
    : RequestHandler<PostHomeInstrumentRequest, PostHomeInstrumentResponse>;

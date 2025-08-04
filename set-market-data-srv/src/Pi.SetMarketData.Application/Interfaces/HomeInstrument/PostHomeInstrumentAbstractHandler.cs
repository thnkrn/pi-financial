using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries;

namespace Pi.SetMarketData.Application.Interfaces.MarketHomeInstrument;

public abstract class PostHomeInstrumentAbstractHandler
    : RequestHandler<PostHomeInstrumentsRequest, PostHomeInstrumentsResponse>;

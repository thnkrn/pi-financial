using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketFilterInstruments;

namespace Pi.SetMarketData.Application.Interfaces.MarketFilterInstruments;

public abstract class PostMarketFilterInstrumentsAbstractHandler
    : RequestHandler<PostMarketFilterInstrumentsRequest, PostMarketFilterInstrumentsResponse>;

using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;


namespace Pi.SetMarketData.Application.Interfaces.MarketInstrumentInfo;

public abstract class GetMarketInstrumentInfoAbstractHandler
    : RequestHandler<PostMarketInstrumentInfoRequest, PostMarketInstrumentInfoResponse>;

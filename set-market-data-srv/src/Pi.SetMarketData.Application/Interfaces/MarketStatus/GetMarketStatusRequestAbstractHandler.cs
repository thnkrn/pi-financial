using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.MarketStatus;

namespace Pi.SetMarketData.Application.Interfaces.MarketStatus;

public abstract class GetMarketStatusRequestAbstractHandler: RequestHandler<GetMarketStatusRequest, GetMarketStatusResponse>;
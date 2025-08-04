using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.TradingSign;

namespace Pi.SetMarketData.Application.Interfaces.TradingSign;

public abstract class GetTradingSignRequestAbstractHandler: RequestHandler<GetTradingSignRequest, GetTradingSignResponse>;
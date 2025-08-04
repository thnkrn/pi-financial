using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.TradingSign;

namespace Pi.SetMarketData.Application.Interfaces.TradingSign;

public abstract class CreateTradingSignRequestAbstractHandler: RequestHandler<CreateTradingSignRequest, CreateTradingSignResponse>;
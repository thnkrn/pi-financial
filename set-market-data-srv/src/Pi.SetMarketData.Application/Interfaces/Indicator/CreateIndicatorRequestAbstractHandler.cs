using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.Indicator;

namespace Pi.SetMarketData.Application.Interfaces.Indicator;

public abstract class CreateIndicatorRequestAbstractHandler: RequestHandler<CreateIndicatorRequest, CreateIndicatorResponse>;
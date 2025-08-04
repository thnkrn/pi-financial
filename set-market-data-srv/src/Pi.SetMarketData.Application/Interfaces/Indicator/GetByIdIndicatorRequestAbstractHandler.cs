using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Indicator;

namespace Pi.SetMarketData.Application.Interfaces.Indicator;

public abstract class GetByIdIndicatorRequestAbstractHandler: RequestHandler<GetByIdIndicatorRequest, GetByIdIndicatorResponse>;
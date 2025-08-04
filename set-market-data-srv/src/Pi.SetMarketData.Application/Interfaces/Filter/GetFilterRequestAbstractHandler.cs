using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.Filter;

namespace Pi.SetMarketData.Application.Interfaces.Filter;

public abstract class GetFilterRequestAbstractHandler: RequestHandler<GetFilterRequest, GetFilterResponse>;
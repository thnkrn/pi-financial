using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Queries.NavList;

namespace Pi.SetMarketData.Application.Interfaces.NavList;

public abstract class GetByIdNavListRequestAbstractHandler: RequestHandler<GetByIdNavListRequest, GetByIdNavListResponse>;
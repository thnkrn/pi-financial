using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Commands.NavList;

namespace Pi.SetMarketData.Application.Interfaces.NavList;

public abstract class DeleteNavListRequestAbstractHandler: RequestHandler<DeleteNavListRequest, DeleteNavListResponse>;
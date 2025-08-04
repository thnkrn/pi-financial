using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.NavList;

public record GetByIdNavListRequest(string id) : Request<GetByIdNavListResponse>;
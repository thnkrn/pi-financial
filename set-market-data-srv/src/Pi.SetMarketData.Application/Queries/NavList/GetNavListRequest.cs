using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.NavList;

public record GetNavListRequest : Request<GetNavListResponse>;
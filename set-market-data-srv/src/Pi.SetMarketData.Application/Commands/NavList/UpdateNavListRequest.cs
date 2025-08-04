using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.NavList;

public record UpdateNavListRequest(string id, Domain.Entities.NavList NavList) : Request<UpdateNavListResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.NavList;

public record CreateNavListRequest(Domain.Entities.NavList NavList) : Request<CreateNavListResponse>;
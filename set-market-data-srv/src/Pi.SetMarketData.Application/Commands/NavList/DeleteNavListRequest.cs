using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.NavList;

public record DeleteNavListRequest(string id) : Request<DeleteNavListResponse>;
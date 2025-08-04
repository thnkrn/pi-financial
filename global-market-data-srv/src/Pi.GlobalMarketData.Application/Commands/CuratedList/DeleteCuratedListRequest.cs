using MassTransit.Mediator;

namespace Pi.GlobalMarketData.Application.Commands;

public record DeleteCuratedListRequest(string id) : Request<DeleteCuratedListResponse>;
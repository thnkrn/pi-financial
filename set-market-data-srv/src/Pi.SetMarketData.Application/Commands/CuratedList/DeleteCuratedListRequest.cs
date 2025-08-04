using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands;

public record DeleteCuratedListRequest(string id) : Request<DeleteCuratedListResponse>;
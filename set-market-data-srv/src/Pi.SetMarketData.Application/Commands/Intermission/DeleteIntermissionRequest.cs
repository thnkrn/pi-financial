using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Intermission;

public record DeleteIntermissionRequest(string id) : Request<DeleteIntermissionResponse>;
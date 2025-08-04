using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Intermission;

public record GetByIdIntermissionRequest(string id) : Request<GetByIdIntermissionResponse>;
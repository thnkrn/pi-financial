using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Intermission;

public record GetIntermissionRequest : Request<GetIntermissionResponse>;
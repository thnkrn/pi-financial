using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Intermission;

public record UpdateIntermissionRequest(string id, Domain.Entities.Intermission Intermission) : Request<UpdateIntermissionResponse>;
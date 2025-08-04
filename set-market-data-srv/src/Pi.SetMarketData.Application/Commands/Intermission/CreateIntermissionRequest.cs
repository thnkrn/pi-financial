using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Intermission;

public record CreateIntermissionRequest(Domain.Entities.Intermission Intermission) : Request<CreateIntermissionResponse>;
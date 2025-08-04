using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.CorporateAction;

public record DeleteCorporateActionRequest(string id) : Request<DeleteCorporateActionResponse>;
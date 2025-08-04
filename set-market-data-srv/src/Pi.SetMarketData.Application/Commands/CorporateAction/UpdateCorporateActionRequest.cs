using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.CorporateAction;

public record UpdateCorporateActionRequest(string id, Domain.Entities.CorporateAction CorporateAction) : Request<UpdateCorporateActionResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.CorporateAction;

public record CreateCorporateActionRequest(Domain.Entities.CorporateAction CorporateAction) : Request<CreateCorporateActionResponse>;
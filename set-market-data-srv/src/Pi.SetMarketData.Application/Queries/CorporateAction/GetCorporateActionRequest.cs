using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.CorporateAction;

public record GetCorporateActionRequest : Request<GetCorporateActionResponse>;
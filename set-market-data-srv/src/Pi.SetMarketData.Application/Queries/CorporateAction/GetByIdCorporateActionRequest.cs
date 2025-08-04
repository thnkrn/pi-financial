using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.CorporateAction;

public record GetByIdCorporateActionRequest(string id) : Request<GetByIdCorporateActionResponse>;
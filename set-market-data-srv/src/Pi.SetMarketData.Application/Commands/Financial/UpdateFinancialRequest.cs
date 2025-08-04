using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Financial;

public record UpdateFinancialRequest(string id, Domain.Entities.Financial Financial) : Request<UpdateFinancialResponse>;
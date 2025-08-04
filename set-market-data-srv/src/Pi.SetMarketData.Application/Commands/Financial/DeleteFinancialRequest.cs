using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Financial;

public record DeleteFinancialRequest(string id) : Request<DeleteFinancialResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.Financial;

public record CreateFinancialRequest(Domain.Entities.Financial Financial) : Request<CreateFinancialResponse>;
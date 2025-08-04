using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Financial;

public record GetFinancialRequest : Request<GetFinancialResponse>;
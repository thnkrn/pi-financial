using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.Financial;

public record GetByIdFinancialRequest(string id) : Request<GetByIdFinancialResponse>;
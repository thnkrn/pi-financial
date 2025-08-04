using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundPerformance;

public record GetByIdFundPerformanceRequest(string id) : Request<GetByIdFundPerformanceResponse>;
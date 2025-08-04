using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Queries.FundPerformance;

public record GetFundPerformanceRequest : Request<GetFundPerformanceResponse>;
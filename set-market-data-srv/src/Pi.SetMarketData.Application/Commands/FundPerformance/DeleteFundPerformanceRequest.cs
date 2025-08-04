using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundPerformance;

public record DeleteFundPerformanceRequest(string id) : Request<DeleteFundPerformanceResponse>;
using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundPerformance;

public record CreateFundPerformanceRequest(Domain.Entities.FundPerformance FundPerformance) : Request<CreateFundPerformanceResponse>;
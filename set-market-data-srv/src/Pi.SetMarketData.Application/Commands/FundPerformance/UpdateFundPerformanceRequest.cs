using MassTransit.Mediator;

namespace Pi.SetMarketData.Application.Commands.FundPerformance;

public record UpdateFundPerformanceRequest(string id, Domain.Entities.FundPerformance FundPerformance) : Request<UpdateFundPerformanceResponse>;
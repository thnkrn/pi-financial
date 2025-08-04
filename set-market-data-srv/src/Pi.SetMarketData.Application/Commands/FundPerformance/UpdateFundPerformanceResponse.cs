namespace Pi.SetMarketData.Application.Commands.FundPerformance;

public record UpdateFundPerformanceResponse(bool Success, Domain.Entities.FundPerformance? UpdatedFundPerformance = null);
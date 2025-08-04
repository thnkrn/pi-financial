namespace Pi.SetMarketData.Application.Commands.FundPerformance;

public record CreateFundPerformanceResponse(bool Success, Domain.Entities.FundPerformance? CreatedFundPerformance = null);
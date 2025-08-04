namespace Pi.SetMarketData.Application.Commands.FundTradeDate;

public record CreateFundTradeDateResponse(bool Success, Domain.Entities.FundTradeDate? CreatedFundTradeDate = null);
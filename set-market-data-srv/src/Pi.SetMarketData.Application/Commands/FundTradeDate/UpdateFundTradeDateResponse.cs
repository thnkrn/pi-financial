namespace Pi.SetMarketData.Application.Commands.FundTradeDate;

public record UpdateFundTradeDateResponse(bool Success, Domain.Entities.FundTradeDate? UpdatedFundTradeDate = null);
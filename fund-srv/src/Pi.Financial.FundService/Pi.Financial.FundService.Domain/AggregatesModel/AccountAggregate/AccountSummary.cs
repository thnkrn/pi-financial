using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.Financial.FundService.Domain.AggregatesModel.AccountAggregate;

public class AccountSummary
{
    public AccountSummary(string customerCode, string tradingAccountNo, DateTime asOfDate, IEnumerable<FundAsset> assets)
    {
        CustomerCode = customerCode;
        TradingAccountNo = tradingAccountNo;
        AsOfDate = asOfDate;
        Assets = assets.Where(asset => asset.TradingAccountNo == tradingAccountNo);
    }

    public string CustomerCode { get; set; }
    public string TradingAccountNo { get; set; }
    public DateTime AsOfDate { get; set; }
    public IEnumerable<FundAsset> Assets { get; set; }

    public decimal TotalMarketValue { get => Assets.Sum(asset => asset.MarketValue); }
    public decimal TotalCostValue { get => Assets.Sum(asset => asset.CostValue); }
    public decimal TotalUpnl { get => decimal.Subtract(TotalMarketValue, TotalCostValue); }
}

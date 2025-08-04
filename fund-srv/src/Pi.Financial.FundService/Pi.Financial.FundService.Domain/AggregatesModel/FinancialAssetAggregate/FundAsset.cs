namespace Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;

public class FundAsset : IAsset, IUpnl
{
    private const decimal Percentage = 100;
    public FundAsset(string fundCode,
        string custCode,
        string tradingAccountNo,
        string unitHolderId)
    {
        FundCode = fundCode;
        UnitHolderId = unitHolderId;
        CustCode = custCode;
        TradingAccountNo = tradingAccountNo;
    }

    public string FundCode { get; }
    public string CustCode { get; }
    public string TradingAccountNo { get; }
    public string UnitHolderId { get; }
    public required decimal Unit { get; init; }
    public required DateOnly AsOfDate { get; init; }
    public required decimal MarketPrice { get; init; }
    public required decimal AvgCostPrice { get; init; }
    public decimal RemainUnit { get; init; }
    public decimal RemainAmount { get; init; }
    public decimal PendingAmount { get; init; }
    public decimal PendingUnit { get; init; }
    public FundInfo? Info { get; private set; }
    public decimal MarketValue { get => decimal.Multiply(MarketPrice, Unit); }
    public decimal CostValue { get => decimal.Multiply(AvgCostPrice, Unit); }
    public decimal UPNL { get => decimal.Subtract(MarketValue, CostValue); }
    public decimal UPNLPercentage
    {
        get
        {
            if (CostValue == 0) return 0;

            return decimal.Multiply(decimal.Divide(UPNL, CostValue), Percentage);
        }
    }

    public bool SetInfo(FundInfo info)
    {
        if (!string.Equals(info.FundCode, FundCode, StringComparison.CurrentCultureIgnoreCase))
        {
            return false;
        }

        Info = info;
        return true;
    }
}

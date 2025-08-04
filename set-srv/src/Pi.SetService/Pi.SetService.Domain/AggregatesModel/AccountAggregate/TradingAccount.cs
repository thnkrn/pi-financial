using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.AggregatesModel.AccountAggregate;

public class TradingAccount
{
    private static readonly TradingAccountType[] SblSupportAccountTypes = [TradingAccountType.CreditBalance];

    public TradingAccount(Guid id, string customerCode, string tradingAccountNo, TradingAccountType tradingAccountType)
    {
        Id = id;
        CustomerCode = customerCode;
        TradingAccountNo = tradingAccountNo;
        AccountNo = TradingAccountNo.Replace("-", "");
        TradingAccountType = tradingAccountType;
    }

    public Guid Id { get; }
    public string CustomerCode { get; }
    public string TradingAccountNo { get; }
    public string AccountNo { get; }
    public TradingAccountType TradingAccountType { get; }
    public bool SblRegistered { get; set; }
    public bool SblEnabled => SblRegistered && SblSupportAccountTypes.Contains(TradingAccountType);
    public IEnumerable<AccountPosition>? Positions { get; private set; }
    public IEnumerable<OnlineOrder>? OpenOrders { get; private set; }

    public void SetPositions(IEnumerable<AccountPosition> positions)
    {
        Positions = positions.Where(q => q.AccountNo == AccountNo);
    }

    public void SetOpenOrders(IEnumerable<OnlineOrder> orders)
    {
        OpenOrders = orders;
    }

    public decimal GetTotalVolumeNvdrStock(string secSymbol)
    {
        return Positions?.Sum(position =>
            position.SecSymbol == secSymbol && position.IsNvdr() ? position.AvailableVolume : 0) ?? 0;
    }

    public decimal GetTotalVolumeNoneNvdrStock(string secSymbol, StockType stockType)
    {
        return Positions?.Sum(position =>
            !position.IsNvdr() && position.SecSymbol == secSymbol && position.StockType == stockType
                ? position.AvailableVolume
                : 0) ?? 0;
    }

    public decimal GetTotalOpenOrderStock(string orderId)
    {
        return OpenOrders != null && OpenOrders.Any() ? OpenOrders.First(order => order.OrderNo.ToString() == orderId).Volume : 0m;
    }
}

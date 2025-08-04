using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class AccountOverview
{
    public string AccountId { get; set; }
    public string TradingAccountNoDisplay { get; set; }
    public string TradingAccountNo { get; set; }
    public Currency Currency { get; private set; }
    public ExchangeRate ExchangeRate { get; private set; }
    public decimal NetAssetValue { get; private set; }
    public decimal MarketValue { get; private set; }
    public decimal Cost { get; private set; }
    public decimal Upnl { get; private set; }

    public decimal UpnlPercentage => Cost == 0m
            ? 0m
            : 100 * Upnl / Cost;

    public decimal TotalBalance => WithdrawableCash + ActiveOrderCash;
    public decimal TradingLimit { get; set; }
    public decimal ActiveOrderCash { get; private set; }
    public decimal WithdrawableCash { get; set; }
    public decimal LineAvailable { get; set; }

    public AccountOverview(Currency currency,
        decimal netAssetValue, decimal marketValue,
        decimal cost, decimal upnl,
        decimal activeOrderCash)
    {
        Currency = currency;
        NetAssetValue = netAssetValue;
        MarketValue = marketValue;
        Cost = cost;
        Upnl = upnl;
        ActiveOrderCash = activeOrderCash;
    }

    public void ChangeCurrency(ExchangeRate exRate)
    {
        if (Currency != exRate.From)
            throw new ArgumentException("Current currency is not valid to exchange");

        ExchangeRate = exRate;

        var rate = exRate.Rate;
        Currency = exRate.To;
        NetAssetValue *= rate;
        MarketValue *= rate;
        Cost *= rate;
        Upnl *= rate;
        TradingLimit *= rate;
        WithdrawableCash *= rate;
        ActiveOrderCash *= rate;
        LineAvailable *= rate;
    }
}

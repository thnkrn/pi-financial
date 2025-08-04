using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Application.Models;

public class AccountInstrumentBalance
{
    public AccountInstrumentBalance(string symbol, AvailableCreditBalance availableBalance,
        IEnumerable<AccountPosition> assets)
    {
        Symbol = symbol;
        Balance = availableBalance.ExcessEquity;

        SetUnit(symbol, assets);
    }

    public AccountInstrumentBalance(string symbol, AvailableCashBalance availableBalance,
        IEnumerable<AccountPosition> assets)
    {
        Symbol = symbol;
        Balance = availableBalance.BuyCredit;

        SetUnit(symbol, assets);
    }

    private void SetUnit(string symbol, IEnumerable<AccountPosition> assets)
    {
        foreach (var asset in assets.Where(q =>
            q.SecSymbol.Equals(symbol, StringComparison.CurrentCultureIgnoreCase) && q.StockType != StockType.Borrow))
        {
            var volume = asset.AvailableVolume;
            if (asset.IsNvdr())
            {
                if (asset.StockType == StockType.Short)
                {
                    ShortNvdrUnit += volume;
                }
                else
                {
                    NvdrUnit += volume;
                }
            }
            else
            {
                if (asset.StockType == StockType.Short)
                {
                    ShortUnit += volume;
                }
                else
                {
                    Unit += volume;
                }
            }
        }
    }

    public decimal Balance { get; init; }
    public string BalanceUnit { get; } = "THB";

    public string Symbol { get; init; }
    public decimal Unit { get; private set; }
    public decimal NvdrUnit { get; private set; }
    public decimal ShortUnit { get; private set; }
    public decimal ShortNvdrUnit { get; private set; }
}

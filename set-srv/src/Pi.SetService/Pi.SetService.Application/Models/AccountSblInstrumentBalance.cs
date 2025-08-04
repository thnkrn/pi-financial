using Pi.SetService.Domain.AggregatesModel.AccountAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Models;

public class AccountSblInstrumentBalance
{
    public AccountSblInstrumentBalance(string symbol, AvailableCreditBalance balance,
        IEnumerable<AccountPosition> positions, TradingDetail tradingDetail, SblInstrumentInfo? sblInstrumentInfo)
    {
        Symbol = symbol;
        ExcessEquity = balance.ExcessEquity;
        PurchesingPower = balance.Pp ?? 0;
        SblEnabled = sblInstrumentInfo != null;
        AvailableLending = sblInstrumentInfo?.SblInstrument.AvailableLending ?? 0;
        ClosePrice = tradingDetail.PrevClose;
        AllowBorrowing = sblInstrumentInfo?.CanBorrow() ?? false;
        MarginRate = sblInstrumentInfo?.MarginInfo.Rate ?? 0;

        var accountPositions = positions as AccountPosition[] ?? positions.ToArray();
        BorrowUnit = accountPositions
            .Where(q => string.Equals(q.SecSymbol, symbol, StringComparison.CurrentCultureIgnoreCase) &&
                        q.StockType == StockType.Borrow).Sum(q => q.AvailableVolume);
        ShortUnit = accountPositions
            .Where(q => string.Equals(q.SecSymbol, symbol, StringComparison.CurrentCultureIgnoreCase) &&
                        q.StockType == StockType.Short).Sum(q => q.AvailableVolume);
    }

    public string Symbol { get; }
    public bool SblEnabled { get; }
    public decimal ExcessEquity { get; }
    public decimal PurchesingPower { get; }
    public decimal AvailableLending { get; }
    public decimal ClosePrice { get; }
    public bool AllowBorrowing { get; }
    public decimal ShortUnit { get; }
    public decimal BorrowUnit { get; }
    public decimal MarginRate { get; }
    public decimal MaximumShares
    {
        get
        {
            var calEe = ClosePrice != 0 ? ExcessEquity / ClosePrice : 0;
            var maximumShares = decimal.Min(AvailableLending, calEe);

            return decimal.Round(maximumShares / 100) * 100;
        }
    }
}

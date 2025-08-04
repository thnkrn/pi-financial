using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class Purchase
{
    public decimal? MinimumInitialBuy { get; init; }
    public decimal? MinimumSubsequentBuy { get; init; }
    public decimal? MinimumSellUnit { get; init; }
    public decimal? MinimumSellAmount { get; init; }
    public decimal? MinHoldUnit { get; init; }
    public decimal? MinHoldAmount { get; init; }
    public int? SellSettlementDay { get; init; }
    public int? BuyCutOffTime { private get; init; }
    public int? SellCutOffTime { private get; init; }
    public DateTime AsOfDate { get; init; }

    private const int DefaultCutOffTime = 1530;

    public DateTime? GetBuyCutOffTimeLocal()
    {
        if (BuyCutOffTime is null or <= 0)
            return null;

        return GetCutOffTimeLocal(BuyCutOffTime.Value);
    }

    public DateTime? GetSellCutOffTimeLocal()
    {
        if (SellCutOffTime is null or <= 0)
            return null;

        return GetCutOffTimeLocal(SellCutOffTime.Value);
    }

    public DateTime GetPiBuyCutOffTimeLocal()
    {
        var cutOffTime = GetBuyCutOffTimeLocal() ?? GetCutOffTimeLocal(DefaultCutOffTime);

        return cutOffTime - StaticConfig.CutOffTimeDeduction;
    }

    public DateTime GetPiSellCutOffTimeLocal()
    {
        var cutOffTime = GetSellCutOffTimeLocal() ?? GetCutOffTimeLocal(DefaultCutOffTime);

        return cutOffTime - StaticConfig.CutOffTimeDeduction;
    }

    private DateTime GetCutOffTimeLocal(int cutOffTime)
    {
        int hours = cutOffTime / 100;
        int minutes = cutOffTime % 100;

        var marketTimeAsUtc = GetMarketTimeLocalAsUtc();

        return new DateTime(marketTimeAsUtc.Year,
            marketTimeAsUtc.Month,
            marketTimeAsUtc.Day,
            hours, minutes, 0);
    }

    protected virtual DateTime GetMarketTimeLocalAsUtc()
    {
        var utcPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var thTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, utcPlus7);

        return thTime;
    }

    public DateTime GetClosestTradeDate(TradeSide side)
    {
        var marketTimeLocal = GetMarketTimeLocalAsUtc();
        var piCutOffTime = side switch
        {
            TradeSide.Buy => GetPiBuyCutOffTimeLocal(),
            TradeSide.Sell => GetPiSellCutOffTimeLocal(),
            TradeSide.Switch => GetPiSellCutOffTimeLocal(),
            _ => throw new ArgumentOutOfRangeException(nameof(side), $"Not expected TradeSide value: {side}.")
        };

        return marketTimeLocal >= piCutOffTime
            ? marketTimeLocal.AddDays(1).Date
            : marketTimeLocal.Date;
    }
}

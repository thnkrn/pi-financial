using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
public class PublicTrade
{
    /// <summary>
    ///     Constructor of PublicTrade
    /// </summary>
    /// <param name="nanos">Unix timestamp, Known as dealDateTime</param>
    /// <param name="dealDateTime">DealDateTime of TickerMessage</param>
    /// <param name="aggressor">Aggressor</param>
    /// <param name="quantity">Volume of trade</param>
    /// <param name="price">Price</param>
    public PublicTrade(long nanos, Timestamp dealDateTime, Alpha? aggressor, Numeric64 quantity, Price32 price)
    {
        Nanos = nanos;
        DealDateTime = dealDateTime;
        Aggressor = aggressor;
        Quantity = quantity;
        Price = price;
    }

    public long Nanos { get; set; }
    public Timestamp DealDateTime { get; set; }
    public Alpha? Aggressor { get; set; }
    public Numeric64 Quantity { get; set; }
    public Price32 Price { get; set; }
    public double PriceChange { get; set; }

    public string GetDealTime()
    {
        TimeZoneInfo bangkokTimeZone;
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Nanos).UtcDateTime;

        try
        {
            // Try IANA ID first (for non-Windows systems)
            bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        }
        catch
        {
            // Fallback to Windows ID
            bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }

        var bangkokTime = TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset, bangkokTimeZone);
        return bangkokTime.ToString("HH:mm:ss");
    }
}

public class PublicTradeResult
{
    /// <summary>
    /// </summary>
    /// <param name="publicTrade"></param>
    public PublicTradeResult(PublicTrade[] publicTrade)
    {
        PublicTrade = publicTrade;
    }

    public PublicTrade[] PublicTrade { get; set; }
}
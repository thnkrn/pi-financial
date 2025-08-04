using System.Globalization;
using Pi.GlobalMarketDataWSS.Application.Helpers;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.Application.Services.FixMapper;

public static class StreamingBodyMapperService
{
    public static StreamingBody Map(StreamingBody streamingBody, Entry entry, string? marketSession, string venue)
    {
        // Debug
        marketSession = MarketSession.PreMarket;
        if (string.IsNullOrEmpty(marketSession))
        {
            return streamingBody;
        }

        streamingBody.IsProjected = marketSession == MarketSession.PreMarket;

        switch (marketSession)
        {
            case MarketSession.PreMarket:
                streamingBody.AuctionPrice = entry.MdEntryPx.ToString(CultureInfo.InvariantCulture);
                streamingBody.AuctionVolume = entry.MdEntrySize.ToString() ?? "0.0";
                break;
            case MarketSession.MainSession:
                streamingBody.PriceChanged = CalculatePriceChanged(
                    entry.MdEntryPx.ToString(CultureInfo.InvariantCulture),
                    streamingBody.PreClose
                );
                streamingBody.PriceChangedRate = CalculatePriceChangeRate(
                    entry.MdEntryPx.ToString(CultureInfo.InvariantCulture),
                    streamingBody.PreClose
                );
                break;
        }

        streamingBody.TotalAmount = CalculateTotalAmount(
            entry.MdEntryPx.ToString(CultureInfo.InvariantCulture),
            streamingBody.TotalVolume
        );
        streamingBody.TotalAmountK = CalculateTotalAmountK(
            entry.MdEntryPx.ToString(CultureInfo.InvariantCulture),
            streamingBody.TotalVolume
        );

        streamingBody.Venue = venue;
        streamingBody.Market = venue;
        streamingBody.Status = marketSession;
        streamingBody.InstrumentType = InstrumentType.GlobalEquity;
        streamingBody.SecurityType = InstrumentType.GlobalEquity;
        streamingBody.LastPriceTime = entry.MdEntryTime.ToString(CultureInfo.InvariantCulture).ToUnixMillisecondTimestamp();
        return streamingBody;
    }

    private static string CalculatePriceChanged(string currentPriceStr, string? closedPriceStr)
    {
        if (string.IsNullOrEmpty(currentPriceStr) || string.IsNullOrEmpty(closedPriceStr))
            return "0.00";

        if (
            double.TryParse(currentPriceStr, out var currentPrice)
            && double.TryParse(closedPriceStr, out var closedPrice)
        )
        {
            var result = Math.Round(currentPrice - closedPrice, 2);

            return result.ToString("F2");
        }
        return "0.00";
    }

    private static string CalculatePriceChangeRate(string currentPriceStr, string? closedPriceStr)
    {
        if (string.IsNullOrEmpty(currentPriceStr) || string.IsNullOrEmpty(closedPriceStr))
            return "0.00";

        if (
            double.TryParse(currentPriceStr, out var currentPrice)
            && double.TryParse(closedPriceStr, out var closedPrice)
            && closedPrice > 0.0f
        )
        {
            var result = Math.Round((currentPrice - closedPrice) / closedPrice * 100, 2);

            return result.ToString("F2");
        }

        return "0.00";
    }

    private static string CalculateTotalAmount(string price, string? totalVolume)
    {
        if (string.IsNullOrEmpty(price) || string.IsNullOrEmpty(totalVolume))
            return "0.00";

        if (
            double.TryParse(price, out var priceValue)
            && double.TryParse(totalVolume, out var totalVolumeValue)
        )
            return (priceValue * totalVolumeValue).ToString("F2");

        return "0.00";
    }

    private static string CalculateTotalAmountK(string price, string? totalVolume)
    {
        if (string.IsNullOrEmpty(price) || string.IsNullOrEmpty(totalVolume))
            return "0.00";

        if (
            double.TryParse(price, out var priceValue)
            && double.TryParse(totalVolume, out var totalVolumeValue)
        )
            return (priceValue * totalVolumeValue / 1000).ToString("F2");

        return "0.00";
    }
}
